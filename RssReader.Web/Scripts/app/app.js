angular.module('rssReaderSPA', []).
    config(['$routeProvider', function($routeProvider) {
        $routeProvider.
        when('/phones', {templateUrl: 'partials/phone-list.html', controller: PhoneListCtrl}).
        when('/phones/:phoneId', {templateUrl: 'partials/phone-detail.html', controller: PhoneDetailCtrl}).
        otherwise({redirectTo: '/phones'});
    }]);


angular.module('rssReaderSPA', [], function ($routeProvider, $locationProvider) {
    $routeProvider.when('/news/:category', {
        templateUrl: '/home/news',
        controller: NewsController
    });

    $routeProvider.when('/configuration', {
        templateUrl: '/home/configuration',
        controller: ConfigurationController
    });

    $routeProvider.otherwise({ redirectTo: '/news/all' });

    // configure html5 to get links working on jsfiddle
    $locationProvider.html5Mode(true);
});
