angular.module("umbraco").controller("UmbracoForms.Editors.Form.CreateController",
	function ($scope, $routeParams, formResource, editorState, notificationsService) {
		formResource.getAllTemplates().then(function(response) {
		    $scope.formTemplates = response.data;
		});
});