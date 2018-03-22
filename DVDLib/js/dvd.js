$(document).ready(function ()
{
    GetAllFilterable();;
    $('#EditDVD').hide();
	

	//Disable Enter Key automatically refreshing the page...
	$("form").submit(function (e) {
		e.preventDefault();
	});
	
    //Call onclick for SearchButton when the user presses the Enter Key and the SearchTerm field has focus
	$('#SearchTerm').keyup(function(event)
	{		
		// Number 13 is the "Enter" key on the keyboard
		if (event.keyCode === 13)
		{
			// Trigger the SearchButton click event
			$('#SearchButton').click();
		}
	});

    $('#AddCancel').click(function ()
	{
		ResetAll();
    });


    $('#EditCancel').click(function ()
	{
		ResetAll();
    });


    $('#DisplayCancel').click(function ()
	{
		ResetAll();
    });
		
    $('#CreateButton').click(function ()
	{
		$('#MainPage').hide();
		$('#AddErrorMessages').empty();
		$('#AddDVD').show();
    });

    $('#AddDVDButton').click(function (event)
	{
        var valid = true;
		
        if ($('#AddTitle').val() == "" )
		{
			$('#AddErrorMessages').append($('<li>').attr({class: 'list-group-item list-group-item-danger'})
			.text('Please enter a title for the DVD.'));
			valid = false;
		}

        var year = $('#AddYear').val();
        if (year.length !=4 || isNaN(year)==true)
		{
			$('#AddErrorMessages').append($('<li>').attr({class: 'list-group-item list-group-item-danger'})
			.text('Please enter a 4-digit year.'));
			valid = false;
        }
		
		if ($('#AddDirector').val() == "" )
		{
			$('#AddErrorMessages').append($('<li>').attr({class: 'list-group-item list-group-item-danger'})
			.text('Please enter a director name.'));
			var valid = false;
        }

        if(valid != true)
		{
			//Invalid data in one or more fields, do not continue...
			return false;
		}

        $.ajax(
		{
            type: 'POST',
            url: 'http://localhost:56915/dvd',
                data: JSON.stringify({
                title: $('#AddTitle').val(),
                realeaseYear: $('#AddYear').val(),
                director: $('#AddDirector').val(),
                rating: $('#AddRating').val(),
                notes: $('#AddNotes').val()
            }),
            headers: {
                'Accept': 'application/json',
                'Content-Type': 'application/json'
            },
            'dataType': 'json',
            success: function(data, status) {
                $('#AddTitle').val('');
                $('#AddYear').val('');
                $('#AddDirector').val('');
                $('#AddRating').val('');
                $('#AddNotes').val('');
                GetAllFilterable();;
				$('#AddErrorMessages').empty();
            },
            error: function() {
                $('#AddErrorMessages').append($('<li>').attr({class: 'list-group-item list-group-item-danger'})
                .text('Error calling REST endpoint.'));
            }
        });
        GetAllFilterable();;
    });

    $('#EditDVDButton').click(function (event)
	{
		var valid = true;
		
		if ($('#EditTitle').val() == "" )
		{
			$('#EditErrorMessages').append($('<li>').attr({class: 'list-group-item list-group-item-danger'})
			.text('Please enter a title for the DVD.'));
			var valid = false;
        }

        var year = $('#EditYear').val();
        if (year.length !=4 || isNaN(year)==true)
		{
			$('#EditErrorMessages').append($('<li>').attr({class: 'list-group-item list-group-item-danger'})
			.text('Please enter a 4-digit year.'));
			var valid = false;
        }
		
		if ($('#EditDirector').val() == "" )
		{
			$('#EditErrorMessages').append($('<li>').attr({class: 'list-group-item list-group-item-danger'})
			.text('Please enter a director name.'));
			var valid = false;
        }

        if(valid != true)
		{
			//Invalid data in one or more fields, do not continue...
			return false;
		}

        $.ajax(
		{
			async: true,
			crossDomain: true,
			type: 'PUT',
			url: 'http://localhost:56915/dvd/' + $('#EditDVDId').val(),
			dataType: 'json',
			processData: false,
			data: JSON.stringify(
			{
				dvdId: $('#EditDVDId').val(),
				title: $('#EditTitle').val(),
				realeaseYear: $('#EditYear').val(),
				director: $('#EditDirector').val(),
				rating: $('#EditRating').val(),
				notes: $('#EditNotes').val(),
			}),
			headers:
			{
				'Accept': 'application/json',
				'Content-Type': 'application/json'
			},
			success: function()
			{
				GetAllFilterable();;
				$('#EditErrorMessages').empty();
			},
			error: function()
			{
				$('#EditErrorMessages').append($('<li>').attr({class: 'list-group-item list-group-item-danger'})
				.text('Error calling REST endpoint.'));
			}
		})
    });

    $('#SearchButton').click(function()
	{
		ClearDVDs();
		ResetAll();

        var searchType = $('#SearchDropDown').val();
        var searchTerm = $('#SearchTerm').val();

        //Check which SearchFilter is selected...
		if(searchType == "AllSearch")
		{
			var searchURL='http://localhost:56915/dvds';
        }
        else if(searchType == "TitleSearch" && searchTerm != null && searchTerm != '')
		{
			var searchURL='http://localhost:56915/dvds/title/' + searchTerm;
        }
        else if(searchType == "YearSearch" && searchTerm != null && searchTerm.length == 4 && isNaN(searchTerm) == false)
		{
			var searchURL='http://localhost:56915/dvds/year/' + searchTerm;
        }
        else if(searchType == "DirectorSearch" && searchTerm != null && searchTerm != '')
		{
			var searchURL='http://localhost:56915/dvds/director/' + searchTerm;
        }
        else if(searchType == "RatingSearch" && searchTerm != null && searchTerm != '')
		{
			var searchURL='http://localhost:56915/dvds/rating/' + searchTerm;
        }
        else
		{
            if(searchType == "YearSearch" && (searchTerm == null || searchTerm.length != 4 || isNaN(searchTerm) == true))
			{
				var errorMessage = "A 4 digit year is required.";				
			}
			else
			{
				var errorMessage = "A search term is required.";				
			}
			//Invalid search term entered, do not continue...
			$('#ErrorMessages').append($('<li>').attr({class: 'list-group-item list-group-item-danger'})
			.text(errorMessage));
			return false;
		}
        GetAllFilterable(searchURL);
    });
})

function GetAllFilterable(filterURL)
{
	ClearDVDs();
    ResetAll();
    $('#ErrorMessages').empty();
	var contentRows = $('#ContentRows');
	
	//If function is called w/o giving a filterURL, set the default one...
	if(filterURL === undefined)
	{
		filterURL = 'http://localhost:56915/dvds';
	}
	
	$.ajax (
	{
        type: 'GET',
        url: filterURL,
        success: function (data, status)
		{
            $.each(data, function (index, dvd)
			{
                var title = dvd.title;
                var releaseYear = dvd.realeaseYear;
                var director = dvd.director;
                var rating = dvd.rating;
                var id = dvd.dvdId;

                var row = '<tr>';
                    row += '<td width="25%"><a onclick="DisplayDVD(' + id + ')">' + title + '</a></td>';
                    row += '<td width="25%">' + releaseYear + '</td>';
                    row += '<td width="15%">' + director + '</td>';
                    row += '<td width="10%">' + rating + '</td>';
                    row += '<td width="5%"><a onclick="LoadEditForm(' + id + ')">Edit</a></td>';
                    row += '<td width="5%"><a onclick="DeleteDVD(' + id + ')">Delete</a></td>';
                    row += '<td width="15%"></td>';
                    row += '</tr>';
					
                contentRows.append(row);

                $('#ErrorMessages').empty();
            });
        },
        error: function()
		{
            $('#ErrorMessages').append($('<li>').attr({class: 'list-group-item list-group-item-danger'})
			.text('Error calling REST endpoint.'));
        }
    });
}

function LoadEditForm(dvdId)
{
    $('#ErrorMessages').empty();
    $('#MainPage').hide();
    $('#AddDVD').hide();
    $('#DisplayDetail').hide();
	$('#EditErrorMessages').empty();
    $('#ErrorMessages').empty();
    $.ajax(
	{
        type: 'GET',
        url: 'http://localhost:56915/dvd/' + dvdId,
        success: function(data, status)
		{
			$('#EditTitle').val(data.title);
			$('#EditYear').val(data.realeaseYear);
			$('#EditDirector').val(data.director);
			$('#EditRating').val(data.rating);
			$('#EditNotes').val(data.notes);
			$('#EditDVDId').val(data.dvdId);

			//Setup the EditRating DropDown to have correct entries
			$('.TempRatingOption').remove();
			if (data.rating != 'G' && data.rating != 'PG' && data.rating != 'PG-13' && data.rating != 'R' && data.rating != 'NC-17')
			{
				$('#EditRating').append('<option class="TempRatingOption" value="'+ data.rating +'" selected="selected">'+ data.rating +'</option>');
			}
			
			$('#ErrorMessages').empty();
        },
        error: function()
		{
            $('#ErrorMessages').append($('<li>').attr({class: 'list-group-item list-group-item-danger'})
		    .text('Error calling REST endpoint.'));
        }
    });
    $('#EditDVD').show();
}

function DeleteDVD(dvdId)
{
	var txt;
	var r = confirm("Are you sure you want to delete this DVD from your collection?");
	if (r == true)
	{
		$.ajax (
		{
			type: 'DELETE',
			url: "http://localhost:56915/dvd/" + dvdId,
			success: function (status)
			{
				GetAllFilterable();;
			}
		});
	}
	else
	{
		GetAllFilterable();;
	}
}

function DisplayDVD(dvdId)
{

	$('#ErrorMessages').empty();
	$('#MainPage').hide();
	$('#DisplayDetail').show();

	$.ajax(
	{
		type: 'GET',
		url: 'http://localhost:56915/dvd/' + dvdId,
		success: function(data, status)
		{
			$('#DisplayTitle').text(data.title);
			$('#DisplayYear').text(data.realeaseYear);
			$('#DisplayDirector').text(data.director);
			$('#DisplayRating').text(data.rating);
			$('#DisplayNotes').text(data.notes);
			$('#ErrorMessages').empty();
		},
		error: function()
		{
			$('#ErrorMessages').append($('<li>').attr({class: 'list-group-item list-group-item-danger'})
			.text('Error calling REST endpoint.'));
		}
	});
}

function ResetAll()
{
	$('#AddDVD').hide();
	$('#EditDVD').hide();
	$('#DisplayDetail').hide();
	$('#ErrorMessages').empty();
	$('#AddErrorMessages').empty();
	$('#EditErrorMessages').empty();
	$('#MainPage').show();
}

function ClearDVDs()
{
    $('#ContentRows').empty();
}

