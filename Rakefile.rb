require 'rake'
require 'albacore'
    
task :default => [:clean_temp, :rake_habanero, :build, :clean_temp]

task :build_FakeBOsInSeperateAssembly => [:clean_FakeBOsInSeperateAssembly,:checkout_FakeBOsInSeperateAssembly,:msbuild_FakeBOsInSeperateAssembly,:copy_dll_to_smooth_lib] 

task :build => [:clean, :updatelib, :build_FakeBOsInSeperateAssembly, :msbuild, :test, :commitlib]

$Nunit_path = "C:/Program Files (x86)/NUnit 2.5.6/bin/net-2.0/nunit-console-x86.exe"
$Nunit_options = '/xml=nunit-result.xml'

$MSBuild_path = "C:/Windows/Microsoft.NET/Framework/v4.0.30319/MSBuild.exe"

$habanero_version = "trunk"

#--------------------------------general tasks-------------------------------------------

task :clean_temp do
	FileUtils.rm_rf 'temp'
end

#--------------------------------build habanero-------------------------------------------

exec :rake_habanero do |cmd|
   cmd.path_to_command = "rake-habanero.cmd"
   cmd.parameters $habanero_version
end


#--------------------------------build FakeBOsInSeperateAssembly-------------------------

task :clean_FakeBOsInSeperateAssembly do 
	FileUtils.rm_rf 'temp/FakeBOsInSeperateAssembly/bin'
end

exec :checkout_FakeBOsInSeperateAssembly do |cmd| 
	cmd.path_to_command = "svn.exe" 
	cmd.parameters %q(checkout "http://delicious:8080/svn/habanero/HabaneroCommunity/SmoothHabanero/trunk/source/FakeBosInSeperateAssembly" temp/FakeBOsInSeperateAssembly/ --username chilli --password chilli --quiet --no-auth-cache) 
end

msbuild :msbuild_FakeBOsInSeperateAssembly do |msb| #builds FakeBOsInSeperateAssembly with msbuild
  msb.targets :Rebuild
  msb.properties :configuration => :Debug
  msb.solution = "temp/FakeBOsInSeperateAssembly/FakeBOsInSeperateAssembly.sln"
  msb.path_to_command = $MSBuild_path
  msb.verbosity = "quiet"
end

task :copy_dll_to_smooth_lib do
	FileUtils.cp Dir.glob('temp/FakeBOsInSeperateAssembly/bin/FakeBosInSeperateAssembly.dll'), 'lib'
end


#-------------------------------build smooth itself ----------------------------------

task :clean do 
	FileUtils.rm_rf 'bin'
end

exec :update_lib_from_svn do |cmd|
	cmd.path_to_command = "svn.exe" 
	cmd.parameters %q(update lib --username chilli --password chilli --quiet --no-auth-cache) 
end

task :updatelib => :update_lib_from_svn do 
	FileUtils.cp Dir.glob('temp/Habanero/bin/Habanero.Base.dll'), 'lib'
	FileUtils.cp Dir.glob('temp/Habanero/bin/Habanero.Base.pdb'), 'lib'
	FileUtils.cp Dir.glob('temp/Habanero/bin/Habanero.Base.xml'), 'lib'
	FileUtils.cp Dir.glob('temp/Habanero/bin/Habanero.BO.dll'), 'lib'
	FileUtils.cp Dir.glob('temp/Habanero/bin/Habanero.BO.pdb'), 'lib'
	FileUtils.cp Dir.glob('temp/Habanero/bin/Habanero.BO.xml'), 'lib'
end

msbuild :msbuild do |msb| 
  msb.targets :Rebuild
  msb.properties :configuration => :Release
  msb.solution = "source/SmoothHabanero_2010.sln"
  msb.verbosity = "quiet"
  msb.path_to_command = $MSBuild_path
end

nunit :test do |nunit|
	nunit.path_to_command = $Nunit_path
	nunit.assemblies 'bin\Habanero.Smooth.Test.dll','bin\Habanero.Naked.Tests.dll', 'bin\Habanero.Fluent.Tests.dll' ,'bin\TestProject.Test.BO.dll','bin\TestProjectNoDBSpecificProps.Test.BO.dll' 
	nunit.options $Nunit_options
end


exec :commitlib do |cmd|
	cmd.path_to_command = "svn.exe" 
	cmd.parameters %q(ci lib -m autocheckin --username chilli --password chilli --quiet --no-auth-cache) 
end