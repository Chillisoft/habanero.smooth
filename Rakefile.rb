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

if (bs.index("branches") == nil)	
	nuget_version = 'Trunk'
	nuget_version_id = '9.9.999'
	
	$nuget_habanero_version	= nuget_version
	
	$nuget_publish_version = nuget_version
	$nuget_publish_version_id = nuget_version_id
else
	$nuget_habanero_version	= 'v2.6-13_02_2012'
	
	$nuget_publish_version = 'v1.6-13_02_2012'
	$nuget_publish_version_id = '1.6'
end		

#------------------------build settings--------------------------
require 'rake-settings.rb'

msbuild_settings = {
  :properties => {:configuration => :release},
  :targets => [:clean, :rebuild],
  :verbosity => :quiet,
  #:use => :net35  ;uncomment to use .net 3.5 - default is 4.0
}

#------------------------dependency settings---------------------

#------------------------project settings------------------------
$solution = 'source/SmoothHabanero_2010.sln'

#______________________________________________________________________________
#---------------------------------TASKS----------------------------------------

desc "Runs the build all task"
task :default => [:build_all_nuget]

desc "Pulls habanero from local nuget, builds and tests smooth"
task :build_all_nuget => [:installNugetPackages, :build, :publishSmoothNugetPackage, :publishNakedNugetPackage]

desc "Builds Smooth, including tests"
task :build => [:clean, :build_FakeBOs, :msbuild, :test]

desc "builds the FakeBOs dll and copies to the lib folder"
task :build_FakeBOs => [:msbuild_FakeBOsInSeperateAssembly,:copy_dll_to_smooth_lib] 

#------------------------build FakeBOsInSeperateAssembly---------

$fakeBOsFolder = "source/FakeBOsInSeperateAssembly"

task :clean_FakeBOsInSeperateAssembly do 
	FileUtils.rm_rf "#{$fakeBOsFolder}/bin"
end

svn :checkout_FakeBOsInSeperateAssembly => :clean_FakeBOsInSeperateAssembly do |s| 
	s.parameters "co #{Dir.pwd}/source/FakeBosInSeperateAssembly #{$fakeBOsFolder}"
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

desc "Builds the solution with msbuild"
msbuild :msbuild do |msb| 
	puts cyan("Building #{$solution} with msbuild")
	msb.update_attributes msbuild_settings
	msb.solution = $solution
end

desc "Runs the tests"
nunit :test do |nunit|
	puts cyan("Running tests")
	nunit.assemblies 'bin\Habanero.Smooth.Test.dll',
					 'bin\Habanero.Naked.Tests.dll', 
					 'bin\Habanero.Fluent.Tests.dll',
					 'bin\TestProject.Test.BO.dll',
					 'bin\TestProjectNoDBSpecificProps.Test.BO.dll' 
end

svn :commitlib do |s|
	puts cyan("Commiting lib")
	s.parameters "ci lib -m autocheckin"
end

desc "Install nuget packages"
getnugetpackages :installNugetPackages do |ip|
    ip.package_names = ["Habanero.Base.#{$nuget_habanero_version}",  
						"Habanero.BO.#{$nuget_habanero_version}"]
end

desc "Publish the Habanero.Smooth nuget package"
pushnugetpackages :publishSmoothNugetPackage do |package|
  package.InputFileWithPath = "bin/Habanero.Smooth.dll"
  package.Nugetid = "Habanero.Smooth.#{$nuget_publish_version}"
  package.Version = $nuget_publish_version_id
  package.Description = "Smooth.Base"
end

desc "Publish the Habanero.Naked nuget package"
pushnugetpackages :publishNakedNugetPackage do |package|
  package.InputFileWithPath = "bin/Habanero.Naked.dll"
  package.Nugetid = "Habanero.Naked.#{$nuget_publish_version}"
  package.Version = $nuget_publish_version_id
  package.Description = "Naked"
end