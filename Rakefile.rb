require 'rake'
require 'albacore'
    
task :default => [:build_habanero,:build_FakeBOsInSeperateAssembly,:build_smooth,:clean_up]#this is the starting point for the script

task :build_habanero => [:clean_habanero,:checkout_habanero,:msbuild_habanero] # a list of tasks spawned off the default task

task :build_FakeBOsInSeperateAssembly => [:clean_FakeBOsInSeperateAssembly,:checkout_FakeBOsInSeperateAssembly,:msbuild_FakeBOsInSeperateAssembly] # a list of tasks spawned off the default task

task :build_smooth => [:clean_smooth,:copy_dlls_to_smooth_lib,:msbuild_smooth,:run_nunit,:commit_lib]

$Nunit_path = "C:/Program Files (x86)/NUnit 2.5.6/bin/net-2.0/nunit-console-x86.exe"
$Nunit_options = '/xml=nunit-result.xml'

#build_habanero tasks
task :clean_habanero do #deletes bin folder
	FileUtils.rm_rf 'temp/Habanero/trunk/bin'
end

exec :checkout_habanero do |cmd| #command to check out habanero source using SVN
	cmd.path_to_command = "../../../Utilities/BuildServer/Subversion/bin/svn.exe" # for some reason this doesn't pick up environment variables so I can't just use 'svn'
	cmd.parameters %q(checkout "http://delicious:8080/svn/habanero/Habanero/trunk" temp/Habanero/trunk/ --username chilli --password chilli) 
end

msbuild :msbuild_habanero do |msb| #builds habanero with msbuild
  msb.targets :Rebuild
  msb.properties :configuration => :Debug
  msb.solution = "temp/Habanero/trunk/source/Habanero.sln"
  msb.path_to_command = "C:/Windows/Microsoft.NET/Framework/v4.0.30319/MSBuild.exe"
  msb.verbosity = "quiet"
  
  end

#build_FakeBOsInSeperateAssembly tasks
task :clean_FakeBOsInSeperateAssembly do #deletes bin folder
	FileUtils.rm_rf 'temp/FakeBOsInSeperateAssembly/bin'
end

exec :checkout_FakeBOsInSeperateAssembly do |cmd| #command to check out FakeBOsInSeperateAssembly source using SVN
	cmd.path_to_command = "../../../Utilities/BuildServer/Subversion/bin/svn.exe" # for some reason this doesn't pick up environment variables so I can't just use 'svn'
	cmd.parameters %q(checkout "http://delicious:8080/svn/habanero/Habanero Community/SmoothHabanero/trunk/source/FakeBosInSeperateAssembly" temp/FakeBOsInSeperateAssembly/ --username chilli --password chilli) 
end

msbuild :msbuild_FakeBOsInSeperateAssembly do |msb| #builds FakeBOsInSeperateAssembly with msbuild
  msb.targets :Rebuild
  msb.properties :configuration => :Debug
  msb.solution = "temp/FakeBOsInSeperateAssembly/FakeBOsInSeperateAssembly.sln"
  msb.path_to_command = "C:/Windows/Microsoft.NET/Framework/v4.0.30319/MSBuild.exe"
  msb.verbosity = "quiet"
end

#build_smooth tasks
task :clean_smooth do #deletes bin folder
	FileUtils.rm_rf 'bin'
end

task :copy_dlls_to_smooth_lib  do #copies habanero DLLs to smooth lib
	FileUtils.cp Dir.glob('temp/Habanero/trunk/bin/Habanero*.dll'), 'lib'
	FileUtils.cp Dir.glob('temp/FakeBOsInSeperateAssembly/bin/FakeBosInSeperateAssembly.dll'), 'lib'
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