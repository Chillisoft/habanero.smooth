require "./rake-tasks/init"
#______________________________________________________________________________
#---------------------------------SETTINGS-------------------------------------

$binaries_baselocation = "bin"
$nuget_baselocation = "nugetArtifacts"
$app_version ='9.9.9.999'

#------------------------build settings--------------------------

msbuild_settings = {
    :properties => {:configuration => :release},
    :targets => [:clean, :rebuild],
    :verbosity => :minimal,
    #:use => :net35  ;uncomment to use .net 3.5 - default is 4.0
}

#------------------------dependency settings---------------------

#------------------------project settings------------------------
$solution = 'source/SmoothHabanero_2010.sln'
$major_version = ''
$minor_version = ''
$patch_version = ''
$nuget_apikey = ''
$nuget_sourceurl = ''
$nuget_publish_version = 'Trunk'
#______________________________________________________________________________
#---------------------------------TASKS----------------------------------------

desc "Runs the build all task"
task :default, [:major, :minor, :patch] => [:setupvars, :build]

desc "Pulls habanero from local nuget, builds, tests and pushes smooth"
task :build_test_push_internal, [:major, :minor, :patch, :apikey, :sourceurl] => [:build_internal, :nugetpush]

desc "Pulls habanero from local nuget, builds and tests smooth"
task :build_internal, [:major, :minor, :patch, :sourceurl] => [:setupvars, :installNugetPackages, :build]

desc "Builds Smooth, including tests"
task :build, [:major, :minor, :patch] => [:clean, :update_packages, :set_assembly_version, :msbuild, :copy_to_nuget, :test]

desc "builds the FakeBOs dll and copies to the lib folder"
task :build_FakeBOs => [:msbuild_FakeBOsInSeperateAssembly, :copy_dll_to_smooth_lib]

#------------------------Setup Versions---------
desc "Setup Variables"
task :setupvars, :major, :minor, :patch, :apikey, :sourceurl do |t, args|
    puts cyan("Setup Variables")
    args.with_defaults(:major => "0")
    args.with_defaults(:minor => "0")
    args.with_defaults(:patch => "0000")
    args.with_defaults(:apikey => "")
    args.with_defaults(:sourceurl => "")
    $major_version = "#{args[:major]}"
    $minor_version = "#{args[:minor]}"
    $patch_version = "#{args[:patch]}"
    $nuget_apikey = "#{args[:apikey]}"
    $nuget_sourceurl = "#{args[:sourceurl]}"
    $app_version = "#{$major_version}.#{$minor_version}.#{$patch_version}.0"
    puts cyan("Assembly Version #{$app_version}")
    puts cyan("Nuget key: #{$nuget_apikey} for: #{$nuget_sourceurl}")
end

desc "Updates packages with nuget"
updatenugetpackages :update_packages do |nuget|
    puts cyan("Updating nuget packages")
    nuget.solution = $solution
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
    FileUtils.rm_rf $nuget_baselocation
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
    FileSystem.ensure_dir_exists "#{location}/"
    FileUtils.cp "#{$binaries_baselocation}/Habanero.Smooth.dll", location
    FileUtils.cp "#{$binaries_baselocation}/Habanero.Smooth.pdb", location
    FileUtils.cp "#{$binaries_baselocation}/Habanero.Naked.dll", location
    FileUtils.cp "#{$binaries_baselocation}/Habanero.Naked.pdb", location
end

task :copy_to_nuget do
    puts cyan("Copying files to the nuget folder")
    copy_nuget_files_to $nuget_baselocation
end

desc "Install nuget packages"
getnugetpackages :installNugetPackages do |ip|
    ip.package_names = ["Habanero.Base.#{$nuget_publish_version}",
                        "Habanero.BO.#{$nuget_publish_version}"]
    ip.SourceUrl = "#{$nuget_sourceurl}/nuget"
    ip.Version = $app_version
end

desc "Pushes Habanero Smooth into the given nuget folder"
task :nugetpush => [:publishSmoothNugetPackage,
                :publishNakedNugetPackage]

desc "Publish the Habanero.Smooth nuget package"
pushnugetpackagesonline :publishSmoothNugetPackage do |package|
    package.InputFileWithPath = ["bin/Habanero.Smooth.dll", "bin/Habanero.Smooth.pdb"]
    package.Nugetid = "Habanero.Smooth.#{$nuget_publish_version}"
    package.Version = $app_version
    package.Description = "Smooth.Base"
    package.ApiKey = "#{$nuget_apikey}"
    package.SourceUrl = "#{$nuget_sourceurl}"
end

desc "Publish the Habanero.Naked nuget package"
pushnugetpackagesonline :publishNakedNugetPackage do |package|
    package.InputFileWithPath = ["bin/Habanero.Naked.dll", "bin/Habanero.Naked.pdb"]
    package.Nugetid = "Habanero.Naked.#{$nuget_publish_version}"
    package.Version = $app_version
    package.Description = "Naked"
    package.ApiKey = "#{$nuget_apikey}"
    package.SourceUrl = "#{$nuget_sourceurl}"
end