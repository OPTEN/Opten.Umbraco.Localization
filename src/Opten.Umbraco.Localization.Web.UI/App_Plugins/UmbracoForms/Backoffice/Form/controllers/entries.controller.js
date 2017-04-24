angular.module("umbraco").controller("UmbracoForms.Editors.Form.EntriesController", function ($scope, $routeParams, recordResource, formResource, dialogService, editorState, userService, securityResource, notificationsService, navigationService) {

    //On load/init of 'editing' a form then
    //Let's check & get the current user's form security
    var currentUserId = null;
    var currentFormSecurity = null;

    //By default set to have access (in case we do not find the current user's per indivudal form security item)
    $scope.hasAccessToCurrentForm = true;

    userService.getCurrentUser().then(function (response) {
        currentUserId = response.id;

        //Now we can make a call to form securityResource
        securityResource.getByUserId(currentUserId).then(function (response) {
            $scope.security = response.data;

            //Use _underscore.js to find a single item in the JSON array formsSecurity 
            //where the FORM guid matches the one we are currently editing (if underscore does not find an item it returns undefinied)
            currentFormSecurity = _.where(response.data.formsSecurity, { Form: $routeParams.id });

            if (currentFormSecurity.length === 1) {
                //Check & set if we have access to the form
                //if we have no entry in the JSON array by default its set to true (so won't prevent)
                $scope.hasAccessToCurrentForm = currentFormSecurity[0].HasAccess;
            }

            //Check if we have access to current form OR manage forms has been disabled
            if (!$scope.hasAccessToCurrentForm || !$scope.security.userSecurity.manageForms) {

                //Show error notification
		        notificationsService.error("Access Denied", "You do not have access to view this form's entries");

                //Resync tree so that it's removed & hides
                navigationService.syncTree({ tree: "form", path: ['-1'], forceReload: true, activate: false }).then(function (response) {

                    //Response object contains node object & activate bool
                    //Can then reload the root node -1 for this tree 'Forms Folder'
                    navigationService.reloadNode(response.node);
                });

                //Don't need to wire anything else up
                return;
            }
        });
    });


	formResource.getByGuid($routeParams.id)
		.then(function(response){
			$scope.form = response.data;
			$scope.loaded = true;

		    //As we are editing an item we can highlight it in the tree
			navigationService.syncTree({ tree: "form", path: [String($routeParams.id), String($routeParams.id) + "_entries"], forceReload: false });

		});

	$scope.states = ["Approved", "Submitted"];

	$scope.filter = {
		startIndex: 1,
		length: 20,
		form: $routeParams.id,
		sortBy: "created",
		sortOrder: "descending",
		states: ["Approved","Submitted"]
	};

	$scope.records = [];

	recordResource.getRecordSetActions().then(function(response){
	    $scope.recordSetActions = response.data;
	    $scope.recordActions = response.data;
	});


	$scope.toggleRecordStateSelection = function(recordState) {
	    var idx = $scope.filter.states.indexOf(recordState);

	    // is currently selected
	    if (idx > -1) {
	        $scope.filter.states.splice(idx, 1);
	    }

	        // is newly selected
	    else {
	        $scope.filter.states.push(recordState);
	    }
	};

	$scope.hiddenFields = [2];
	$scope.toggleSelection = function toggleSelection(field) {
	    var idx = $scope.hiddenFields.indexOf(field);

	    // is currently selected
	    if (idx > -1) {
	      $scope.hiddenFields.splice(idx, 1);
	    }else {
	      $scope.hiddenFields.push(field);
	    }
	};


	$scope.edit = function(schema){
	    dialogService.open(
	            {
	                template: "/app_plugins/UmbracoForms/Backoffice/Form/dialogs/entriessettings.html",
	                schema: schema,
	                toggle: $scope.toggleSelection,
	                hiddenFields: $scope.hiddenFields,
					filter: $scope.filter
	            });
	};

	$scope.viewdetail = function(schema, row){
		dialogService.open(
				{
					template: "/app_plugins/UmbracoForms/Backoffice/Form/dialogs/entriesdetail.html",
					schema: schema,
					row: row,
					hiddenFields: $scope.hiddenFields
				});
	};

	$scope.pagination = [];

	$scope.next = function(){
		$scope.filter.startIndex++;
	};

	$scope.prev = function(){
		$scope.filter.startIndex--;
	};

	$scope.gotoPage = function(index){
		$scope.filter.startIndex = index;
	};


	$scope.search = _.debounce(function(resetIndex){

		$scope.reset(resetIndex);

		$scope.$apply(function(){
			recordResource.getRecords($scope.filter).then(function(response){
				$scope.records = response.data;
				$scope.pagination.length = $scope.records.totalNumberOfPages;
			});
		});


	}, 300);


	$scope.$watch("filter", function(newVal, oldVal) {
	
		var resetIndex = newVal.filter !== oldVal.filter;
		$scope.search(resetIndex);

	}, true);


	$scope.loadRecords = function(filter, append){
		recordResource.getRecords(filter).then(function(response){
			if(append){
				$scope.records = $scope.records.results.concat(response.data.results);
			}else{
				$scope.records = response.data;
			}

			$scope.allIsChecked =  ($scope.selectedRows.length >= $scope.records.results.length);
		});
	};

	$scope.reset = function(resetIndex){
		$scope.selectedRows.length = 0;
		$scope.allIsChecked = false;

		if(resetIndex){
			$scope.filter.startIndex = 1;
		}

	};

	$scope.more = function(){
		$scope.filter.startIndex++;
		$scope.loadRecords($scope.filter, true);
	};

	$scope.selectedRows = [];
	$scope.toggleRow = function(row){
		if(row.selected){
			$scope.selectedRows.push(row.id);
			$scope.allIsChecked =  ($scope.selectedRows.length >= $scope.records.results.length);
		}else{
			var i = $scope.selectedRows.indexOf(row.id);
			$scope.selectedRows.splice(i,1);
			$scope.allIsChecked = false;
		}
	};

	$scope.allIsChecked = false;
	$scope.toggleAll = function($event){
		var checkbox = $event.target;
		$scope.selectedRows.length = 0;

		for (var i = 0; i < $scope.records.results.length; i++) {
			var entity = $scope.records.results[i];
			entity.selected = checkbox.checked;

			if(checkbox.checked){
				$scope.selectedRows.push(entity.id);
			}
		}
	};

	$scope.executeRecordSetAction = function (action) {

        //Get the data we need in order to send to the API Endpoint
	    var model = {
	        formId: $scope.form.id,
	        recordKeys: $scope.selectedRows,
	        actionId: action.id
	    };

	    //Check if the action we are running requires a JS Confirm Box along with a message to be displayed
	    if (action.needsConfirm && action.confirmMessage.length > 0) {

	        //Display the confirm box with the confirmMessage
	        var result = confirm(action.confirmMessage);

	        if (!result) {
	            //The user clicked cancel
	            //Stop the rest of the function running
	            return;
	        }
	    }

	    //We do not need to show a confirm message so excute the action immediately
	    recordResource.executeRecordSetAction(model).then(function (response) {
	        $scope.reset(true);
	        $scope.loadRecords($scope.filter, false);

	        //Show success notification that action excuted
	        notificationsService.success("Excuted Action", "Successfully excuted action " + action.name);

	    }, function (err) {
	        //Error Function - so get an error response from API
	        notificationsService.error("Excuted Action", "Failed to excute action " + action.name + " due to error: " + err);
	    });

	    
	};
});
