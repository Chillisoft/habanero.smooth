require 'rake'
require 'albacore'

#______________________________________________________________________________
#---------------------------------SETTINGS-------------------------------------

#------------------------build settings--------------------------
class Svn
  include Albacore::Task
  include Albacore::RunCommand
  def execute
	@command = "svn.exe"
    run_command "svn", "--username chilli --password chilli --quiet --no-auth-cache"
  end
end

msbuild_settings = {
  :properties => {:configuration => :release},
  :targets => [:clean, :rebuild],
  :verbosity => :quiet
}

Albacore.configure do |config|
  config.log_level = :quiet
  config.nunit do |nunit|
    nunit.command = "C:/Program Files (x86)/NUnit 2.5.6/bin/net-2.0/nunit-console-x86.exe"
    nunit.options = ["/xml=nunit-result.xml"]
  end
end

#------------------------dependency settings---------------------
require 'rake-habanero.rb'
$habanero_version = 'trunk'

#------------------------project settings------------------------
$basepath = 'http://delicious:8080/svn/habanero/HabaneroCommunity/SmoothHabanero/trunk'
$solution = 'source/SmoothHabanero_2010.sln'

#______________________________________________________________________________
#---------------------------------TASKS----------------------------------------

task :default => [:build_all]
task :build_all => [:clean_temp, :rake_habanero, :build, :clean_temp]
task :build => [:clean, :updatelib, :build_FakeBOsInSeperateAssembly, :msbuild, :test, :commitlib]
task :build_FakeBOsInSeperateAssembly => [:msbuild_FakeBOsInSeperateAssembly,:copy_dll_to_smooth_lib] 

#------------------------general tasks---------------------------

task :clean_temp do
	FileUtils.rm_rf 'temp'
end

#------------------------build FakeBOsInSeperateAssembly---------

$fakeBOsFolder = "temp/FakeBOsInSeperateAssembly"

task :clean_FakeBOsInSeperateAssembly do 
	FileUtils.rm_rf "#{$fakeBOsFolder}/bin"
end

svn :checkout_FakeBOsInSeperateAssembly => :clean_FakeBOsInSeperateAssembly do |s| 
	s.parameters "co #{$basepath}/source/FakeBosInSeperateAssembly #{$fakeBOsFolder}"
end

msbuild :msbuild_FakeBOsInSeperateAssembly => :checkout_FakeBOsInSeperateAssembly do |msb| 
	msb.update_attributes msbuild_settings
    msb.solution = "#{$fakeBOsFolder}/FakeBOsInSeperateAssembly.sln"
end

task :copy_dll_to_smooth_lib do
	FileUtils.cp Dir.glob("#{$fakeBOsFolder}/bin/FakeBosInSeperateAssembly.dll"), 'lib'
end

#------------------------build smooth itself --------------------

task :clean do 
	FileUtils.rm_rf 'bin'
end

svn :update_lib_from_svn do |s|
	s.parameters "update lib"
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
	msb.update_attributes msbuild_settings
	msb.solution = $solution
end

nunit :test do |nunit|
	nunit.assemblies 'bin\Habanero.Smooth.Test.dll','bin\Habanero.Naked.Tests.dll', 'bin\Habanero.Fluent.Tests.dll' ,'bin\TestProject.Test.BO.dll','bin\TestProjectNoDBSpecificProps.Test.BO.dll' 
end

svn :commitlib do |s|
	s.parameters "ci lib -m autocheckin"
end