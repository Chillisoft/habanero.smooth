require 'rake'
require 'albacore'

#______________________________________________________________________________
#---------------------------------SETTINGS-------------------------------------

# set up the build script folder so we can pull in shared rake scripts.
# This should be the same for most projects, but if your project is a level
# deeper in the repo you will need to add another ..
bs = File.dirname(__FILE__)
bs = File.join(bs, "..") if bs.index("branches") != nil
bs = File.join(bs, "../HabaneroCommunity/BuildScripts")
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

$binaries_baselocation = "bin"
$nuget_baselocation = "nugetArtifacts"
$app_version ='9.9.9.999'
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
$major_version = ''
$minor_version = ''
$patch_version = ''
#______________________________________________________________________________
#---------------------------------TASKS----------------------------------------

desc "Runs the build all task"
task :default, [:major, :minor, :patch] => [:build_all_nuget]

desc "Pulls habanero from local nuget, builds and tests smooth"
task :build_all_nuget, [:major, :minor, :patch] => [:installNugetPackages, :setupversion, :set_assembly_version, :build, :copy_to_nuget, :publishSmoothNugetPackage, :publishNakedNugetPackage]

desc "Builds Smooth, including tests"
task :build, [:major, :minor, :patch] => [:clean, :setupversion, :set_assembly_version, :build_FakeBOs, :msbuild, :copy_to_nuget, :test]

desc "builds the FakeBOs dll and copies to the lib folder"
task :build_FakeBOs => [:msbuild_FakeBOsInSeperateAssembly,:copy_dll_to_smooth_lib] 

#------------------------Setup Versions---------
desc "Setup Versions"
task :setupversion,:major ,:minor,:patch do |t, args|
	puts cyan("Setup Versions")
	args.with_defaults(:major => "0")
	args.with_defaults(:minor => "0")
	args.with_defaults(:patch => "0000")
	$major_version = "#{args[:major]}"
	$minor_version = "#{args[:minor]}"
	$patch_version = "#{args[:patch]}"
	$app_version = "#{$major_version}.#{$minor_version}.#{$patch_version}.0"
	puts cyan("Assembly Version #{$app_version}")	
end

task :set_assembly_version do
	puts green("Setting Shared AssemblyVersion to: #{$app_version}")
	file_path = "source/Common/AssemblyInfoShared.cs"
	outdata = File.open(file_path).read.gsub(/"9.9.9.999"/, "\"#{$app_version}\"")
	File.open(file_path, 'w') do |out|
		out << outdata
	end	
end
#------------------------build FakeBOsInSeperateAssembly---------

$fakeBOsFolder = "source/FakeBOsInSeperateAssembly"

task :clean_FakeBOsInSeperateAssembly do 
	FileUtils.rm_rf "#{$fakeBOsFolder}/bin"
end

msbuild :msbuild_FakeBOsInSeperateAssembly do |msb| 
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

def copy_nuget_files_to location
	FileUtils.cp "#{$binaries_baselocation}/Habanero.Smooth.dll", location
	FileUtils.cp "#{$binaries_baselocation}/Habanero.Naked.dll", location
end

task :copy_to_nuget do
	puts cyan("Copying files to the nuget folder")	
	copy_nuget_files_to $nuget_baselocation
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