angular.module('rssReader.filters', []).
    config(function ($filterProvider) {

        $filterProvider.register('excludeExistingTags', function () {
            
            return function (all, existing) {

                if (all) {
                    var arrayToReturn = [];

                    for (var j = 0; j < all.length; j++) {

                        if (existing.indexOf(all[j]) == -1) {
                            arrayToReturn.push(all[j]);
                        };
                    }

                    return arrayToReturn;
                }
            };
        });       
    });