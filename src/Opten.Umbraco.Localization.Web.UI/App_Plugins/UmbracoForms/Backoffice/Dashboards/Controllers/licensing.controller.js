angular.module("umbraco")
.controller("UmbracoForms.Dashboards.LicensingController",
    function ($scope, $location, $routeParams, $cookieStore, formResource, licensingResource, updatesResource, notificationsService) {
        
        $scope.overlay = {
            show: false,
            title: "Congratulations",
            description: "You've just installed Umbraco Forms - Let's create your first form"
        };

        var packageInstall = $cookieStore.get("umbPackageInstallId");
        if(packageInstall){
            $scope.overlay.show = true; 
            $cookieStore.put("umbPackageInstallId", ""); 
        }

        //if not initial install, but still do not have forms
        if(!$scope.overlay.show){
            formResource.getOverView().then(function(response){
                if(response.data.length === 0){
                    $scope.overlay.show = true;
                    $scope.overlay.title = "Create a form";
                    $scope.overlay.description = "You do not have any forms setup yet, how about creating one now?";
                }    
            });
        }

        $scope.getLicenses = function(config){

            $scope.loginError = false;
            $scope.hasLicenses = undefined;

            licensingResource.getAvailableLicenses(config).then(function(response){
                var licenses = response.data;
                var currentDomain = window.location.hostname;

                $scope.hasLicenses = licenses.length > 0;
                _.each(licenses, function(lic){
                    if(lic.bindings && lic.bindings.indexOf(currentDomain) >= 0){
                        lic.currentDomainMatch = true;
                    }
                });

                $scope.configuredLicenses = _.filter(licenses, function(license){ return license.configured; });
                $scope.openLicenses = _.filter(licenses, function(license){ return license.configured === false; });

            }, function(err){
                $scope.loginError = true;
                $scope.hasLicenses = undefined;
            });
        };


        $scope.configure = function(config){
            licensingResource.configureLicense(config).then(function(response){
                $scope.configuredLicenses.length = 0;
                $scope.openLicenses.length = 0;
                $scope.loadStatus();

                notificationsService.success("License configured", "Umbraco forms have been configured to be used on this website");
            });
        };

        $scope.loadStatus = function(){
            licensingResource.getLicenseStatus().then(function(response){
                    $scope.status = response.data;
            });

            updatesResource.getUpdateStatus().then(function(response){
                $scope.version = response.data;
            });

            updatesResource.getVersion().then(function (response) {
                $scope.currentVersion = response.data;
            });


        };

        $scope.upgrade = function(){
            $scope.installing = true;
            updatesResource.installLatest($scope.version.remoteVersion).then(function(response){
                window.location.reload();
            });
        };


        $scope.create= function(){
            $location.url("forms/form/edit/-1?template=&create=true");
        };


        $scope.configuration = {domain: window.location.hostname};
        $scope.loadStatus();
    });
