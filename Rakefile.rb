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
$buildscriptpath = File.expand_path(bs)
$:.unshift($buildscriptpath) unless
    $:.include?(bs) || $:.include?($buildscriptpath)

#------------------------build settings--------------------------
require 'rake-settings.rb'

msbuild_settings = {
  :properties => {:configuration => :release},
  :targets => [:clean, :rebuild],
  :verbosity => :quiet,
  #:use => :net35  ;uncomment to use .net 3.5 - default is 4.0
}

#------------------------dependency settings---------------------
$habanero_version = 'branches/v2.5'
require 'rake-habanero.rb'


#------------------------project settings------------------------
$basepath = 'http://delicious:8080/svn/habanero/HabaneroCommunity/SmoothHabanero/branches/v1.3'
$solution = 'source/SmoothHabanero_2008.sln'

#______________________________________________________________________________
#---------------------------------TASKS----------------------------------------

desc "Runs the build all task"
task :default => [:build_all]

desc "Rakes habanero, builds Smooth"
task :build_all => [:create_temp, :rake_habanero, :build, :delete_temp]

desc "Builds Smooth, including tests"
task :build => [:clean, :updatelib, :build_FakeBOs, :msbuild, :test, :commitlib]

desc "builds the FakeBOs dll and copies to the lib folder"
task :build_FakeBOs => [:msbuild_FakeBOsInSeperateAssembly,:copy_dll_to_smooth_lib] 

#------------------------build FakeBOsInSeperateAssembly---------

$fakeBOsFolder = "temp/FakeBOsInSeperateAssembly"

task :clean_FakeBOsInSeperateAssembly do 
	FileUtils.rm_rf "#{$fakeBOsFolder}/bin"
end

svn :checkout_FakeBOsInSeperateAssembly => :clean_FakeBOsInSeperateAssembly do |s| 
	s.parameters "co #{$basepath}/source/FakeBosInSeperateAssembly #{$fakeBOsFolder}"
end

msbuild :msbuild_FakeBOsInSeperateAssembly => :checkout_FakeBOsInSeperateAssembly do |msb| 
	puts cyan("building FakeBOsInSeperateAssembly in #{$fakeBOsFolder}")
	msb.update_attributes msbuild_settings
    msb.solution = "#{$fakeBOsFolder}/FakeBOsInSeperateAssembly.sln"
end

task :copy_dll_to_smooth_lib do
	FileUtils.cp Dir.glob("#{$fakeBOsFolder}/bin/FakeBosInSeperateAssembly.dll"), 'lib'
end

#------------------------build smooth itself --------------------

desc "Cleans the bin folder"
task :clean do
	puts cyan("Cleaning bin folder")
	FileUtils.rm_rf 'bin'
end

svn :update_lib_from_svn do |s|
	s.parameters "update lib"
end

task :updatelib => :update_lib_from_svn do 
	puts cyan("Updating lib")
	FileUtils.cp Dir.glob('temp/bin/Habanero.Base.dll'), 'lib'
	FileUtils.cp Dir.glob('temp/bin/Habanero.Base.pdb'), 'lib'
	FileUtils.cp Dir.glob('temp/bin/Habanero.Base.xml'), 'lib'
	FileUtils.cp Dir.glob('temp/bin/Habanero.BO.dll'), 'lib'
	FileUtils.cp Dir.glob('temp/bin/Habanero.BO.pdb'), 'lib'
	FileUtils.cp Dir.glob('temp/bin/Habanero.BO.xml'), 'lib'
end

desc "Builds the solution with msbuild"
msbuild :msbuild do |msb| 
	puts cyan("Building #{$solution} with msbuild")
	msb.update_attributes msbuild_settings
	msb.solution = $solution
end

desc "Runs the tests"
nunit :test do |nunit|
	puts cyan("Running tests")
	nunit.assemblies 'bin\Habanero.Smooth.Test.dll','bin\Habanero.Naked.Tests.dll'
end

svn :commitlib do |s|
	#puts cyan("Commiting lib")
	#s.parameters "ci lib -m autocheckin"
end