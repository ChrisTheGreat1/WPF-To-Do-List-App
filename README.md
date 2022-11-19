# WPF To-Do List App

This app implements a basic to-do list creator, weather checking and basic personal email functions. 

This is a project I created to become acquainted with WPF and XAML files. It is a general proof of concept project to show experience in C# WPF, so only basic functionalities are implemented as this is not intended for everyday real-world usage. UI/UX features are primitive as my main focus was on the backend coding. 

![Start Menu](/ToDo_App/Images/Github_StartMenu.PNG)

## TO-DO LIST

- Provides user with a rich text editor & calendar date picker to create to-do list items 
- To-do list items are saved to a local SQLite database. Options also exist to save them to computer as a rich text file, or to immediately send them as an email or SMS
- Editing and delete functions for existing to-do list items

![To-Do List Menu](/ToDo_App/Images/Github_CreateEditDelete.PNG)

![To-Do List Editor](/ToDo_App/Images/Github_Editor.PNG)

![To-Do List Item Viewer](/ToDo_App/Images/Github_ViewItem.PNG)



## WEATHER

- Provides user with an interface to enter a city and country then display information such as current temperature, windspeed, sunrise/sunset, weather conditions. 
- Information is obtained via API call to openweathermap.org (https://openweathermap.org/api)

![Weather](/ToDo_App/Images/Github_Weather.PNG)

## EMAIL

- Provide user with an interface to login/logout of their personal email (currently only supports Outlook/Hotmail emails that do not have 2FA enabled)
- Retrieves user's email inbox and displays selectable list of all emails
- Email reply functionality with built-in text editor

![Email Login](/ToDo_App/Images/Github_EmailLogin.PNG)

![Email Inbox](/ToDo_App/Images/Github_EmailInbox.PNG)

![Email Reply](/ToDo_App/Images/Github_EmailReply.PNG)

## MISC. COMMENTS

- Some of the SQLite database implementation could be considered inefficient, as a call is made by the app to reload the entire database into memory everytime an item is created/edited/deleted. A more efficient solution could be obtained through implementation of Entity Framework, however I wanted to use this project as an opportunity to learn Dapper and the System.Data libraries. 
- Async methods are often written then called at the conclusion of the button click event handlers without using the "await" keyword. This is to avoid requiring "async void" return types for the event handlers - more info on this topic can be found [here](https://learn.microsoft.com/en-us/archive/msdn-magazine/2013/march/async-await-best-practices-in-asynchronous-programming)

