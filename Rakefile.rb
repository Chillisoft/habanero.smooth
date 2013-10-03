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
  :verbosity => :quiet
  
}

#------------------------dependency settings---------------------

#------------------------project settings------------------------
$basepath = 'http://delicious:8080/svn/habanero/HabaneroCommunity/SmoothHabanero/branches/v1.5_CF_Stargate'
$solution = 'source/SmoothHabanero_2008_CF.sln'
#______________________________________________________________________________
#---------------------------------TASKS----------------------------------------

desc "Runs the build all task"
task :default => [:build_all]

# desc "builds only, no building of habanero"
# task :build_smooth_only => [:create_temp, :build, :delete_temp]

desc "Rakes habanero, builds Smooth"
task :build_all => [:create_temp, :build, :delete_temp, :nuget]

desc "Builds Smooth, including tests"
task :build => [:clean, :installNugetPackages, :build_FakeBOs, :msbuild, :test]

desc "builds the FakeBOs dll and copies to the lib folder"
task :build_FakeBOs => [:msbuild_FakeBOsInSeperateAssembly,:copy_dll_to_smooth_lib] 

desc "Pushes Habanero into the local nuget folder"
task :nuget => [:publishSmoothNugetPackage ]


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

desc "Install nuget packages"
getnugetpackages :installNugetPackages do |ip|
   ip.package_names = ["Habanero.Base.v2.6-CF_Stargate", 
						"Habanero.BO.v2.6-CF_Stargate"]
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
	nunit.assemblies 'bin\Habanero.Smooth.Test.dll','bin\TestProject.Test.BO.dll','bin\TestProjectNoDBSpecificProps.Test.BO.dll' 
end

desc "Publish the Habanero.Smooth nuget package"
pushnugetpackages :publishSmoothNugetPackage do |package|
  package.InputFileWithPath = "bin/Habanero.Smooth.dll"
  package.Nugetid = "Habanero.Smooth.v1.5_CF_Stargate"
  package.Version = "1.5"
  package.Description = "Habanero.Smooth"
end
