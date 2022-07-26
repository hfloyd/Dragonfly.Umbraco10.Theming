﻿angular.module("umbraco")
    .controller("CssOverridePickerController",
    function ($scope, umbRequestHelper, $http) {

    //var url = Umbraco.Sys.ServerVariables["Theming"]["ThemingPropertyEditorsBaseUrl"] + "GetThemes";
        var url = "/Umbraco/backoffice/Dragonfly/CssOverridePropertyEditor/GetFiles";

    umbRequestHelper.resourcePromise(
            $http.get(url),
            'Failed to retrieve CSS files for Site at ' + url)
        .then(function (data) {
            $scope.themes = data;
        });

});