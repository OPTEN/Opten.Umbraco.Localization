angular.module("umbraco").controller("UmbracoForms.SettingTypes.File",
	function ($scope, dialogService) {

	    $scope.openMediaPicker = function() {
	        dialogService.mediaPicker({ callback: populateFile });
	    };

        function populateFile(item) {
            $scope.setting.value = item.properties[0].value;
        }
	});