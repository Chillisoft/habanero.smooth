require 'rake'
require 'albacore'

#______________________________________________________________________________
#---------------------------------SETTINGS-------------------------------------

# set up the build script folder so we can pull in shared rake scripts.
# This should be the same for most projects, but if your project is a level
# deeper in the repo you will need to add another ..
bs = File.dirname(__FILE__)
bs = File.join(bs, "..") if bs.index("branches") != nil
bs = File.join(bs, "../../../HabaneroCommunity/BuildScripts")
$:.unshift(File.expand_path(bs)) unless
    $:.include?(bs) || $:.include?(File.expand_path(bs))

#------------------------build settings--------------------------
require 'rake-settings.rb'

msbuild_settings = {
  :properties => {:configuration => :release},
  :targets => [:clean, :rebuild],
  :verbosity => :quiet,
  #:use => :net35  ;uncomment to use .net 3.5 - default is 4.0
}

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