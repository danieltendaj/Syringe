---
title: Viewing results
layout: page
menu:
  parent: main
---
On the header menu you have two options:

 1. All results
 2. Today's results

They both show the same information except today's results shows results only from today whereas all results shows results up to a year.

## Filter Results

By default results from all environments will be shown. 

In order to change this you must do the following:

 1. Click on the "Environment" Button
 2. Choose the environment from the drop down to get an updated view of the results.

## Paging
Results are limited to 20 per page. You can change this by adding/changing the noOfResults parameter in the query string to a different number. We will probably add this as a feature later on. 

### Example script block (remove me)

	#sidebar-wrapper::-webkit-scrollbar {
	  display: none;
	}

	#wrapper.toggled #sidebar-wrapper {
	    width: 220px;
	}

	#page-content-wrapper {
	    width: 100%;
	    padding-top: 70px;
	}

	#wrapper.toggled #page-content-wrapper {
	    position: absolute;
	    margin-right: -220px;
	}