angular.module('rssReaderSPA', ['http-loading-interceptor', 'ui'], function ($routeProvider, $locationProvider, $provide) {
    $routeProvider.when('/news/:category', {
        templateUrl: '/partials/news',
        controller: NewsController
    });

    $routeProvider.when('/suscriptions', {
        templateUrl: '/partials/suscriptions',
        controller: SuscriptionController
    });

    $routeProvider.otherwise({ redirectTo: '/news/all' });

    // configure html5 to get links working on jsfiddle
    $locationProvider.html5Mode(true);

    $provide.factory('userService', function ($http) {
        alert('user service constructor');

        return {
            get: function (callback) {
                $http.get('/api/user/getuser').success(function (data) {
                    // prepare data here
                    callback(data);
                });
            }
        };

    });
})
.filter('excludeExistingTags', function(){

    return function (all, existing) {

        var arrayToReturn = [];

        for (var j = 0; j < all.length; j++) {

            if (existing.indexOf(all[j]) == -1) {
                arrayToReturn.push(all[j]);
            };
        }

        return arrayToReturn;
    };
});

/*
taken from:
http://www.34m0.com/2012/12/angularjs-and-spinjs-in-coffeescript.html
*/
angular.module('spinner', []).provider('spinner', function () {
    this.$get = function () {
        return null;
    };
    this.startSpinner = function () {
        var data, spinnerEl;
        spinnerEl = $('<p id="spinner"></p>');
        data = spinnerEl.data();
        if (data.spinner) {
            data.spinner.stop();
            delete data.spinner;
        }
        spinnerEl.appendTo('body');
        return data.spinner = new Spinner().spin(spinnerEl.get(0));
    };
    return this.stopSpinner = function () {
        var data, spinnerEl;
        spinnerEl = $('#spinner');
        data = spinnerEl.data();
        if (data.spinner) {
            data.spinner.stop();
            delete data.spinner;
        }
        return spinnerEl.remove();
    };
});


angular.module('http-loading-interceptor', ['spinner']).config([
  '$httpProvider', 'spinnerProvider', function ($httpProvider, spinnerProvider) {
      var interceptor;
      interceptor = function (data, headersGetter) {
          spinnerProvider.startSpinner();
          return data;
      };
      return $httpProvider.defaults.transformRequest.push(interceptor);
  }
]).config([
  '$httpProvider', 'spinnerProvider', function ($httpProvider, spinnerProvider) {
      var interceptor;
      interceptor = [
        '$q', '$window', function ($q, $window) {
            var error, success;
            success = function (response) {
                spinnerProvider.stopSpinner();
                return response;
            };
            error = function (response) {
                spinnerProvider.stopSpinner();
                return $q.reject(response);
            };
            return function (promise) {
                return promise.then(success, error);
            };
        }
      ];
      return $httpProvider.responseInterceptors.push(interceptor);
  }
]);