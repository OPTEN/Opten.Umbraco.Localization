angular.module("umbraco").controller("UmbracoForms.Editors.Form.EntriesSettingsController",
    function($scope, $log, $timeout, exportResource){

        exportResource.getExportTypes().then(function(response){
            $scope.exportTypes = response.data;
        });

        $scope.export = function(type, filter){
            filter.exportType = type.id;
            exportResource.generateExport(filter).then(function(response){

                var url = exportResource.getExportUrl(response.data.file);
                var iframe = document.createElement('iframe');
                iframe.id = "hiddenDownloadframe";
                iframe.style.display = 'none';
                document.body.appendChild(iframe);
                iframe.src= url;

                //remove all traces
                $timeout(function(){
                    document.body.removeChild(iframe);
                }, 1000);
            });
        };

    });
