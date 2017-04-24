angular.module("umbraco").controller("UmbracoForms.SettingTypes.Pickers.ContentWithXpathController",
	function ($scope, $routeParams, dialogService, entityResource, iconHelper) {

	if (!$scope.setting) {
	    $scope.setting = {};
	}

	var val = parseInt($scope.setting.value);
	

	if (!isNaN(val) && angular.isNumber(val)) {
	    //node
	    $scope.showQuery = false;

	    entityResource.getById($scope.setting.value, "Document").then(function (item) {
	        item.icon = iconHelper.convertFromLegacyIcon(item.icon);
	        $scope.node = item;
	    });
	} else {
	    //xpath
	    $scope.showQuery = true;
	    $scope.query = $scope.setting.value;
	}

	$scope.openContentPicker = function () {
	    var d = dialogService.treePicker({
	        section: "content",
	        treeAlias: "content",
	        multiPicker: false,
	        callback: populate
	    });
	};

	$scope.setXpath = function() {
	    $scope.setting.value = $scope.query;
	};

	$scope.clear = function () {
	    $scope.id = undefined;
	    $scope.node = undefined;
	    $scope.setting.value = undefined;
	};

	function populate(item) {
	    $scope.clear();
	    item.icon = iconHelper.convertFromLegacyIcon(item.icon);
	    $scope.node = item;
	    $scope.id = item.id;
	    $scope.setting.value = item.id;
	}

});