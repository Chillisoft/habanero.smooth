#this file is used to build habanero as a dependency. It utilises another rake script found in the repo to 
#do the build. So you need an svn task defined as well as:
#   $habanero_version - the version - that is, the folder in the repo from which to get the script

task :rake_habanero => [:checkout_buildscript, :write_script, :do_rake, :delete_script]

$buildscript_path = 'http://delicious:8080/svn/habanero/HabaneroCommunity/BuildScripts'
$rakehabanero = 'rake-habanero.cmd'

svn :checkout_buildscript do |s|
	s.parameters "co #{$buildscript_path}/habanero/#{$habanero_version} temp/habanero"
end

task :write_script do
	File.open $rakehabanero, "w" do | f | 
		f.write "cd temp/habanero \n"
		f.write "rmdir /s /q .svn \n"
		f.write "rake --rakefile habanero-library-rakefile.rb \n"
	end
end
	   
exec :do_rake do |cmd|
   cmd.command = $rakehabanero
   cmd.parameters $habanero_version
end
	   
task :delete_script do 
   FileUtils.rm $rakehabanero
end

