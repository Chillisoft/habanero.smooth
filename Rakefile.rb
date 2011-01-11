require 'rake'
require 'albacore'
    
task :default => [:build_habanero,:build_smooth,:clean_up]#this is the starting point for the script

task :build_habanero => [:clean_habanero,:checkout_habanero,:msbuild_habanero] # a list of tasks spawned off the default task

task :build_smooth => [:clean_smooth,:copy_dlls_to_smooth_lib,:msbuild_smooth,:run_nunit]

$Nunit_path = "C:/Program Files (x86)/NUnit 2.5.6/bin/net-2.0/nunit-console-x86.exe"
$Nunit_options = '/xml=nunit-result.xml'

#build_habanero tasks
task :clean_habanero do #deletes bin folder
	FileUtils.rm_rf 'temp/Habanero/trunk/bin'
end

exec :checkout_habanero do |cmd| #command to check out habanero source using SVN
	cmd.path_to_command = "../../../../Utilities/BuildServer/Subversion/bin/svn.exe" # for some reason this doesn't pick up environment variables so I can't just use 'svn'
	cmd.parameters %q(checkout "http://delicious:8080/svn/habanero/Habanero/trunk" temp/Habanero/trunk/ --username chilli --password chilli) 
end

msbuild :msbuild_habanero do |msb| #builds habanero with msbuild
  msb.targets :Rebuild
  msb.properties :configuration => :Debug
  msb.solution = "temp/Habanero/trunk/source/Habanero.sln"
end

#build_smooth tasks
task :clean_smooth do #deletes bin folder
	FileUtils.rm_rf 'bin'
end

task :copy_dlls_to_smooth_lib  do #copies habanero DLLs to smooth lib
	FileUtils.cp Dir.glob('temp/Habanero/trunk/bin/Habanero*.dll'), 'lib'
end

msbuild :msbuild_smooth do |msb| #builds smooth with msbuild
  msb.targets :Rebuild
  msb.properties :configuration => :Release
  msb.solution = "source/SmoothHabanero_2010.sln"
  msb.verbosity = "quiet"
  end

nunit :run_nunit do |nunit|
	nunit.path_to_command = $Nunit_path
	nunit.assemblies 'bin\Habanero.Smooth.Test.dll' #this cannot be passed into the task as a global variable, unfortunately as it seems to be an array of sorts
	nunit.options $Nunit_options
end

task :clean_up do
FileUtils.rm_rf 'temp'
end