; (function () {
	'use strict';

	angular.module("umbraco").controller("OPTEN.Backoffice.UrlAlias.Controller", urlAliasController);

	urlAliasController.$inject = ["$scope", "$rootScope", "editorState", "languageResource"];

	function urlAliasController($scope, $rootScope, editorState, languageResource) {
		var vm = this;

		vm.loadings = [];

		vm.getIndex = getIndex;

		// We set a scope with the value otherwise we have a "empty" JSON saved.
		$scope.urls = $scope.model.value || [];

		// Legacy fix if json doesn't contains "name"
		legacy();

		$scope.states = {
			isEnabled: false,
			hasHostnames: false,
			isPublished: false,
			hasLanguages: false
		};

		$scope.$watchCollection("states", function (newValue, oldValue) {
			enable(newValue);
		});

		$rootScope.$watchCollection("languages", function (newValue, oldValue) {
			if (newValue && newValue.length && vm.loadings.length < 2) {
				vm.loadings.push("languages");

				$scope.states.hasLanguages = newValue.length > 0;
			}
		});

		$scope.$watch('urls', function (newValue, oldValue) {

			var errors = [];

			for (var index = 0; index < newValue.length; index++) {
				var segment = newValue[index].url;
				newValue[index].hasError = false;
				validateValidity();

				// check if the current url has the same segment
				if (editorState.current.urls && editorState.current.urls.length) {
					var currentSegment = editorState.current.urls[0].split('/');
					currentSegment = currentSegment[currentSegment.length - 2];

					setError(newValue[index], segment.toLowerCase() === currentSegment.toLowerCase());
				}

				// Then if other in the same node has the same url
				angular.forEach(newValue, function (language, i) {
					if (index != i) {
						setError(newValue[index], language.url.toLowerCase() === segment.toLowerCase());
					}
				});

				(function (index2) {
					languageResource.getUrlAvailability(editorState.current.id, segment, getNodeLevel())
						.then(function (isAvailable) {
							setError(newValue[index2], (isAvailable.toString().toLowerCase() == "false"));
						});
				}(index));
				

				function setError(language, hasError) {
					if (hasError) {
						language.hasError = true;
					}
					validateValidity();
				};
			}

		}, true);

		activate();

		$scope.$on("formSubmitting", function () {
			//TODO: If we have a valid state the json get's created but could be "empty"
			// don't know if this is a problem or we have to check each input if it has a value
			// and then create the json? => this code here don't work if it's a directive...

			if ($scope.states.isPublished == false || ($scope.states.isEnabled && $scope.states.hasHostnames)) {
				// Delete all urls if has hostnames!
				$scope.model.value = null;
			}
			else if ($scope.urls && $scope.urls.length) {
				// Check if we have any localized url
				// Only set the JSON to the $scope.model.value if we really have something.
				if (hasAnyLocalized()) {
					$scope.model.value = $scope.urls;
				}
				else {
					$scope.model.value = null;
				}
			}
		});


		// Private functions

		function activate() {

			//TODO: Is there a way to get the hostnames on the node by angular?
			// so we don't have to pass in the current node id

			var contentId = !editorState.current ? 0 : editorState.current.id;

			//TODO: Only call this when published?
			languageResource.getState(contentId).then(function (state) {
				vm.loadings.push("isEnabled");
				vm.loadings.push("hasHostnames");

				$scope.states.isEnabled = state.isEnabled;
				$scope.states.hasHostnames = state.hasHostnames;
			});

			$scope.states.isPublished = editorState.current.published;
		};

		function enable(state) {
			// We wait until everything is loaded
			if (state.isPublished && vm.loadings.length >= 3) {

				// Only set the languages if the states are valid
				if (state.isEnabled &&
					state.hasHostnames == false &&
					state.isPublished &&
					state.hasLanguages) {
					setLanguages($rootScope.languages);
				}
				else {
					$scope.model.value = null;
				}
			}
		};

		function validateValidity() {
			var isValid = $scope.urls.filter(function (url) {
				return url.hasError;
			}).length === 0;

			$scope.urlAliasForm.$setValidity("isValid", isValid);
		}

		function setLanguages(languages) {
			// We have to set the json with the languages if never set
			// or add the new one's but don't delete the old ones
			var found = false;
			for (var i = 0; i < languages.length; i++) {
				found = false;

				for (var y = 0; y < $scope.urls.length; y++) {
					if (languages[i].isoCode == $scope.urls[y].isoCode) {
						found = true;
						break;
					}
				}

				if (found == false) {
					$scope.urls.push({
						isoCode: languages[i].isoCode,
						url: '',
						name: '',
						hasError: false
					});
				}
			}

		};

		function getNodeLevel() {
			if (!editorState.current.path || editorState.current.path == null) return -1;
			return (editorState.current.path.split(',').length - 1);
		};

		function getIndex(isoCode) {
			for (var i = 0; i < $scope.urls.length; i++) {
				if ($scope.urls[i].isoCode.toLowerCase() == isoCode.toLowerCase()) {
					return i;
				}
			}

			return -1;
		};

		function hasAnyLocalized() {
			var anyLocalized = false;
			for (var i = 0; i < $scope.urls.length; i++) {
				if ($scope.urls[i].hasError == false &&
					$scope.urls[i].url != "") {
					anyLocalized = true;
					break;
				}
			}
			return anyLocalized;
		};

		function legacy() {
			if ($scope.urls.length) {
				angular.forEach($scope.urls, function (language, i) {
					if (!$scope.urls[i].name) {
						$scope.urls[i].name = $scope.urls[i].url;
					}
				});
			}
		};

	};

}());