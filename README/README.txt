Goal:

Be able to load CKFinder images from an S3 bucket and set the `root` common prefix for loading from that bucket dynamically
instead of using a static string for `root`.



Environment Setup:

1.) Add `127.0.0.1 	local.cktester.com` to host file

2.) Create a new website in IIS, pointed it to this code folder, assign it to the binding `local.cktester.com`

3.) In IIS, for the newly created website, right click on the `ckfinder3` folder and click `Convert to
    Application`; choose default settings on prompt; ensure the App Pool for this website is using version 4+ for
    it's .NET CLR version

4.)  If needed, add the `.json` MIME type `application/json` to this website in IIS as instructed on
     https://ckeditor.com/docs/ckfinder/ckfinder3-net/quickstart.html#quickstart_installation_zip

5.) in the file `/CKFinderDemo/js/ckfinder3/src/CKFinder.Connector.WebApp/App_Code/ConnectorConfig.cs`, starting
    at line 50, enter working S3 credentials and bucket name

6.) Launch `http://local.cktester.com/` in browser of choice

7.) See how the Editor/Finder are invoked in the index.html file starting at line 51; we attempt to `pass` a value named
    `appGuid` to the connector there



Reproduce Issues(s):

1.) Load `http://local.cktester.com/` (using the supplied default Web.config and `ConnectorConfig.cs` where S3 credentials
    were just added; check the web inspector and see the `ckfinder.js` file load throwing an error (see issue_01.jpg)

2.) Rename the supplied default Web.config to some other name; rename the file from `/js/ckfinder3/Web.config.CUSTOMRESOURCES`
    to `/js/ckfinder3/Web.config`; (this file is the same at the last, but has no resourceTypes defined); in the
    `ConnectorConfig.cs` file, un-comment the `CUSTOM RESOURCE` portion of code; RE-load the page `http://local.cktester.com/`
    and note the error loading the `ckfinder.js` again (see issue_02.jpg)



Working Example:

1.) Rename `/js/ckfinder3/Web.config.WORKING` to `/js/ckfinder3/Web.config` (rename current Web.config as necessary); To the
    new `Web.config` file, enter working S3 credentials and bucket name (starting on line 43); Comment out both the
    `CUSTOM BACKEND` and `CUSTOM BACKEND` sections in `ConnectorConfig.cs`; reload the page at `http://local.cktester.com/`
    (see working_01.jpg and working_02.jpg)



Bonus Issues:

1.) Maybe related to other issues, but when using the Working Example, and choosing one of the uploaded images to place in the
    editor body, the image cannot be loaded in the body (see bonus_01.jpg and bonus_02.jpg)
