; (function () {
	'use strict';

	angular.module("umbraco.directives").directive("optenPropertyMigration", propertyMigration);

	propertyMigration.$inject = ["OPTEN.Backoffice.Localization.Migration.Resource", "$route"];

	function propertyMigration(localizationMigrationResource, $route) {
		var directive = {
			restrict: "A",
			link: link,
			scope: {
				tab: '=tab',
				contentType: '=contentType',
				property: '=property'
			}
		};

		return directive;

		function link(scope, element, attr) {
			element.bind('click', function () {
				if (confirm("Do you want to localize this property?")) {
					var property = {
						alias: scope.property.alias,
						contentTypeAlias: scope.contentType.alias,
						dataTypeDefinitionId: scope.property.dataTypeId,
						group: scope.tab.name,
						localize: true,
						name: scope.property.label,
						propertyEditorAlias: scope.property.editor

					}

					localizationMigrationResource.postProperty(property).then(function () {
						$route.reload();
					});
				}
			});
		};
	};
}());