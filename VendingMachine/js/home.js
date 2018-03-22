$(document).ready(function(){

    var calcChange = true; //flag indicates if change gets calculated and returned on reset with no purchase
    var totalAmount = 0;

    //Set initial values
    init();

    $('#txtTotalAmount').val('$'+totalAmount.toFixed(2));

    $('#btnAddDollar').on('click', function() {
        incrementTotalAmount(1.00);
    });

    $('#btnAddQuarter').on('click', function() {
        incrementTotalAmount(.25); 
    });

    $('#btnAddDime').on('click', function() {
        incrementTotalAmount(.10);    
    });

    $('#btnAddNickel').on('click', function(){
        incrementTotalAmount(.05);
    });

    $('#btnCoinReturn').on('click', function() {
        if(calcChange)
        {
            calculateChange();
            totalAmount = 0;
            $('#txtTotalAmount').val('$'+totalAmount.toFixed(2));
            $('#txtItemMessage').val('Reset!!! Press Change Return...')
            $('#txtItemSelected').val('');
            $('#btnCoinReturn').empty();
            $('#btnCoinReturn').append('Change Return');
            $('#btnAddDollar').attr("disabled","disabled");
            $('#btnAddQuarter').attr("disabled","disabled");
            $('#btnAddDime').attr("disabled","disabled");
            $('#btnAddNickel').attr("disabled","disabled");
            $('#btnItemPurchase').attr("disabled","disabled");
            calcChange = false;
        }
        else
        {
            init();
            $('#btnAddDollar').removeAttr("disabled");
            $('#btnAddQuarter').removeAttr("disabled");
            $('#btnAddDime').removeAttr("disabled");
            $('#btnAddNickel').removeAttr("disabled");
            $('#btnItemPurchase').removeAttr("disabled"); 
        }

    });

    $('#btnItemPurchase').on('click', function(){
        if($('#txtItemSelected').val()==""){
            $('#txtItemMessage').val('You must select an item first!');
        }
        else{
            //Do Ajax call for vending the item!
            var myURL = 'http://localhost:8080/money/'+totalAmount.toFixed(2)+'/item/'+$('#txtItemSelected').val();
            $.ajax({
                type: 'GET',
                url: myURL,
                success: function(data) {
                    //evaluate the return item values building a string to set the value of txtReturnAmount
                    var strChange='';
                    if (data.quarters == 1) {strChange += data.quarters + ' Quarter '}
                    if (data.quarters > 1) {strChange += data.quarters + ' Quarters '}
                    if (data.dimes == 1) {strChange += data.dimes + ' Dime '}
                    if (data.dimes > 1) {strChange += data.dimes + ' Dimes '}
                    if (data.nickels == 1) {strChange += data.nickels + ' Nickel '}
                    if (data.nickels > 1) {strChange += data.nickels + ' Nickels '}
                    if (data.pennies == 1) {strChange += data.pennies + ' Penny '}
                    if (data.pennies > 1) {strChange += data.pennies + ' Pennies '}
                    calcChange = false; //Set a flag since change is already calculated and shown
                    $('#txtCoinAmounts').val(strChange);
                    $('#txtItemMessage').val('Thank You!!! Press Change Return...');
                    $('#btnCoinReturn').empty();
                    $('#btnCoinReturn').append('Change Return');
                    $('#btnAddDollar').attr("disabled","disabled");
                    $('#btnAddQuarter').attr("disabled","disabled");
                    $('#btnAddDime').attr("disabled","disabled");
                    $('#btnAddNickel').attr("disabled","disabled");
                    $('#btnItemPurchase').attr("disabled","disabled");
                },
                error: function(xhr, errorType, exception) {
                    var err = jQuery.parseJSON(xhr.responseText);
                    $('#txtItemMessage').val(err.message);
                }
            });
        };

        //Disabled refreshing here, conflicted with spec.  Happens after Change Return is pressed now...

        ////Now update the items
        //$('#VM_Items').empty();
        //loadItems();
    });
    
    function loadItems(){
        var myURL = 'http://localhost:8080/items';
        $.ajax({
            type: 'GET',
            url: myURL,
            success: function(itemArray) {
            // get a reference to the 'VM_Items' div
            var itemsDiv = $('#VM_Items');
            // go through each of the returned items and append the info to the itemsDiv
            $.each(itemArray, function(index, item) {
                var itemInfo = '<div class=\"btn btn-default btn-sq-lg\" style=\"border:double\" id=\"VM_Item' + item.id + '\" onclick=\"setItem(' + item.id + ')\">'; 
                itemInfo += '<div class="btn-sq-content">';
                itemInfo += '<p class=\"text-left\" name=\"id\">' + item.id + '</p>';
                itemInfo += '<p class=\"text-center\">' + item.name + '</p>';
                itemInfo += '<p class=\"text-center\">$' + item.price + '</p><br/>';
                itemInfo += '<p class=\"text-center\">Quantity Left:' + item.quantity + '</p>';
                itemInfo += '</div>';
                itemInfo += '</div>';
                itemsDiv.append(itemInfo);
                })
            }, 
            error: function(xhr, errorType, exception) {
                $('#txtItemMessage').val('Can not connect to REST Endpoint!!!');
            }
        });
    };

    function init(){
        //Resets the Vending Machine to Initial State
        calcChange = true; //set flag so change gets calculated and returned on reset with no purchase
        totalAmount = 0;
        $('#txtTotalAmount').val('$'+totalAmount.toFixed(2));
        $('#txtItemMessage').val('Select an item...');
        $('#txtItemSelected').val('');
        $('#txtCoinAmounts').val('');
        $('#btnCoinReturn').empty();
        $('#btnCoinReturn').append('Reset');
        $('#VM_Items').empty();  //Disable this to test item formatting with test item divs in HTML
        loadItems();
    }

    function calculateChange(){
        var strChangeAmount = '';
        var numberCoins = 0;

        numberCoins = Math.floor(totalAmount.toFixed(2) / 0.25);
        totalAmount = totalAmount.toFixed(2) - numberCoins * 0.25;
        if (numberCoins == 1) strChangeAmount += ' ' + numberCoins + ' Quarter';
        if (numberCoins > 1) strChangeAmount += ' ' + numberCoins + ' Quarters';
        numberCoins = Math.floor(totalAmount.toFixed(2) / 0.10);
        totalAmount = totalAmount.toFixed(2) - numberCoins * 0.10;
        if (numberCoins == 1) strChangeAmount += ' ' + numberCoins + ' Dime';
        if (numberCoins > 1) strChangeAmount += ' ' + numberCoins + ' Dimes';
        numberCoins = Math.floor(totalAmount.toFixed(2) / 0.05);
        totalAmount = totalAmount.toFixed(2) - numberCoins * 0.05;
        if (numberCoins == 1) strChangeAmount += ' ' + numberCoins + ' Nickel';
        if (numberCoins > 1) strChangeAmount += ' ' + numberCoins + ' Nickels';
        //Not implementing pennies, since there currently is no way to put in pennies...

        if (strChangeAmount.length > 0)
        {
            $('#txtCoinAmounts').val(strChangeAmount);
        }
        else
        {
            $('#txtCoinAmounts').val('');
        }
    }

    function incrementTotalAmount(increment){
        totalAmount += increment;
        $('#txtTotalAmount').val('$'+totalAmount.toFixed(2));
    }

})

function setItem(itemId){
    $("#txtItemSelected").val(itemId);
    $("#txtItemMessage").val("");
}

