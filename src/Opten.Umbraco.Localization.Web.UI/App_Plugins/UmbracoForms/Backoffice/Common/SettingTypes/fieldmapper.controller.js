angular.module("umbraco").controller("UmbracoForms.SettingTypes.FieldMapperController",
	function ($scope, $routeParams, pickerResource) {

	    if (!$scope.setting.value) {
	        $scope.mappings = [];
	    } else {
	        $scope.mappings = JSON.parse($scope.setting.value);
	    }

        pickerResource.getAllFields($routeParams.id).then(function (response) {
            $scope.fields = response.data;
        });

        $scope.addMapping = function () {
            $scope.mappings.push({
                alias: $scope.alias,
                value: $scope.value,
                staticValue: $scope.staticValue
        });
	        $scope.alias = '';
	        $scope.value = '';
	        $scope.staticValue = '';
	        $scope.setting.value = JSON.stringify($scope.mappings);
        };

	    $scope.deleteMapping = function(index) {
	        $scope.mappings.splice(index, 1);
	        $scope.setting.value = JSON.stringify($scope.mappings);
	    };
	});