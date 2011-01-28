@cd temp/habanero 
@rmdir /s /q .svn 
rake --rakefile habanero-library-rakefile.rb --execute-continue "$habanero_version = 'branches/v2.5'" 
