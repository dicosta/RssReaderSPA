angular.module('rssReader.services', []).
    config(function($provide) {
        $provide.factory('userService', function ($http) {
            
            //alert('one-time code from user-service');

            return {
                getFeeds: function (callback) {

                    //alert('get feeds method');

                    $http.get('/api/user/getuser').success(function (data) {                        
                        callback(data.Feeds);
                    });

                },

                getUserId: function (callback) {                    
                    //alert('get user Id method');
                },

                getTags: function (callback) {
                    $http.get('/api/user/getuser').success(function (data) {
                        callback(data.Tags);
                    });
                }
            };
        });

        $provide.factory('tagService', function ($http) {

            return {
                addTag: function (tagName, callback) {

                    $http({
                        method: 'POST',
                        url: 'api/user/category',
                        data: $.param({ "": tagName }),
                        headers: { 'Content-Type': 'application/x-www-form-urlencoded' }
                    }).success(function (data) {
                    });

                },

                removeTag: function (tagName, callback) {
                    $http({
                        method: 'DELETE',
                        url: 'api/user/category/' + tagName,
                    }).success(function (data) {
                    });
                }
            };

        });
                
        $provide.factory('newsService', function ($http) {

            return {
                getNews : function(callback) {

                    $http.get('/api/new/getnews').success(function (data) {
                        callback(data);
                    });
                },

                getNewsByTag : function(tag, callback) {
                    $http.get('/api/new/getnewsbytag/' + tag).success(function (data) {
                        callback(data);
                    });
                },

                tagNew: function (newId, tag) {
                    $http({
                        method: 'POST',
                        url: '/api/new/tagNew',
                        data: { NewId: newId, TagName: tag }
                    }).success(function (data) {
                    });
                },

                untagNew: function (newId, tag) {
                    $http({
                        method: 'POST',
                        url: '/api/new/unTagNew',
                        data: { NewId: newId, TagName: tag }
                    }).success(function (data) {
                    });
                }
            };
        });
    });