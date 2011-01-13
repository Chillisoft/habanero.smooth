require 'rake'
require 'albacore'
    
task :default => [:rake_habanero,:build_smooth,:clean_up]

task :build_FakeBOsInSeperateAssembly => [:clean_FakeBOsInSeperateAssembly,:checkout_FakeBOsInSeperateAssembly,:msbuild_FakeBOsInSeperateAssembly,:copy_dll_to_smooth_lib] 

task :build_smooth => [:clean_smooth,:copy_habanero_dlls_to_smooth_lib,:build_FakeBOsInSeperateAssembly,:msbuild_smooth,:run_nunit,:commit_lib]

$Nunit_path = "C:/Program Files (x86)/NUnit 2.5.6/bin/net-2.0/nunit-console-x86.exe"
$Nunit_options = '/xml=nunit-result.xml'

exec :rake_habanero do |cmd|
   cmd.path_to_command = "rake-habanero.cmd"
end

#build_FakeBOsInSeperateAssembly tasks
task :clean_FakeBOsInSeperateAssembly do #deletes bin folder
	FileUtils.rm_rf 'temp/FakeBOsInSeperateAssembly/bin'
end

exec :checkout_FakeBOsInSeperateAssembly do |cmd| #command to check out FakeBOsInSeperateAssembly source using SVN
	cmd.path_to_command = "../../../Utilities/BuildServer/Subversion/bin/svn.exe" # for some reason this doesn't pick up environment variables so I can't just use 'svn'
	cmd.parameters %q(checkout "http://delicious:8080/svn/habanero/HabaneroCommunity/SmoothHabanero/trunk/source/FakeBosInSeperateAssembly" temp/FakeBOsInSeperateAssembly/ --username chilli --password chilli) 
end

msbuild :msbuild_FakeBOsInSeperateAssembly do |msb| #builds FakeBOsInSeperateAssembly with msbuild
  msb.targets :Rebuild
  msb.properties :configuration => :Debug
  msb.solution = "temp/FakeBOsInSeperateAssembly/FakeBOsInSeperateAssembly.sln"
  msb.path_to_command = "C:/Windows/Microsoft.NET/Framework/v4.0.30319/MSBuild.exe"
  msb.verbosity = "quiet"
end

task :copy_dll_to_smooth_lib do
	FileUtils.cp Dir.glob('temp/FakeBOsInSeperateAssembly/bin/FakeBosInSeperateAssembly.dll'), 'lib'
end

#build_smooth tasks
task :clean_smooth do #deletes bin folder
	FileUtils.rm_rf 'bin'
end

task :copy_habanero_dlls_to_smooth_lib  do #copies habanero DLLs to smooth lib
	FileUtils.cp Dir.glob('temp/Habanero/bin/Habanero.Base.dll'), 'lib'
	FileUtils.cp Dir.glob('temp/Habanero/bin/Habanero.Base.pdb'), 'lib'
	FileUtils.cp Dir.glob('temp/Habanero/bin/Habanero.Base.xml'), 'lib'
	FileUtils.cp Dir.glob('temp/Habanero/bin/Habanero.BO.dll'), 'lib'
	FileUtils.cp Dir.glob('temp/Habanero/bin/Habanero.BO.pdb'), 'lib'
	FileUtils.cp Dir.glob('temp/Habanero/bin/Habanero.BO.xml'), 'lib'
	
end


msbuild :msbuild_smooth do |msb| #builds smooth with msbuild
  msb.targets :Rebuild
  msb.properties :configuration => :Release
  msb.solution = "source/SmoothHabanero_2010.sln"
  msb.verbosity = "quiet"
  msb.path_to_command = "C:/Windows/Microsoft.NET/Framework/v4.0.30319/MSBuild.exe"
  end

nunit :run_nunit do |nunit|
	nunit.path_to_command = $Nunit_path
	nunit.assemblies 'bin\Habanero.Smooth.Test.dll','bin\Habanero.Naked.Tests.dll', 'bin\Habanero.Fluent.Tests.dll' ,'bin\TestProject.Test.BO.dll','bin\TestProjectNoDBSpecificProps.Test.BO.dll' #this cannot be passed into the task as a global variable, unfortunately as it seems to be an array of sorts
	nunit.options $Nunit_options
end

task :clean_up do
FileUtils.rm_rf 'temp'
end

exec :commit_lib do |cmd| #command to check out habanero source using SVN
	cmd.path_to_command = "../../../Utilities/BuildServer/Subversion/bin/svn.exe" # for some reason this doesn't pick up environment variables so I can't just use 'svn'
	cmd.parameters %q(ci -m autocheckin --username chilli --password chilli) 
end