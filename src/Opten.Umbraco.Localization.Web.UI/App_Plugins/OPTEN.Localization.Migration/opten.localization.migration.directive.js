; (function () {
	'use strict';

	angular.module("umbraco.directives").directive("optenPropertyMigration", propertyMigration);

	propertyMigration.$inject = ["OPTEN.Backoffice.Localization.Migration.Resource", "$route"];

	function propertyMigration(localizationMigrationResource, $route) {
		var directive = {
			restrict: "E",
			controller: controller,
			replace: true,
			templateUrl: '/App_Plugins/OPTEN.Localization.Migration/directives/propertyMigration.html?v=ASSEMBLY_VERSION',
			scope: {
				tab: '=tab',
				contentType: '=contentType',
				property: '=property'
			}
		};

		return directive;

		function controller($scope) {
			$scope.property.localize = false;

			$scope.togglePrompt = function () {
				$scope.property.localize = !$scope.property.localize;
			}

			$scope.hidePrompt = function () {
				$scope.property.localize = false;
			}

			$scope.localize = function () {
				var property = {
					alias: $scope.property.alias,
					contentTypeAlias: $scope.contentType.alias,
					dataTypeDefinitionId: $scope.property.dataTypeId,
					group: $scope.tab.name,
					localize: true,
					name: $scope.property.label,
					propertyEditorAlias: $scope.property.editor
				}

				localizationMigrationResource.postProperty(property).then(function () {
					$route.reload();
				});
			}
		}
	};
}());