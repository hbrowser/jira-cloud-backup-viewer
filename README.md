# jira-cloud-backup-viewer
Within Jira you can search for issues and comments using a free text search. This allows for easily looking up previous solutions to problems.

If you want to remove projects or entire subscriptions from Jira, this search option is no longer available. Jira does not allow for an easily  readable export that can be consulted independently.

JiraCloudBackupViewer aims to provide a way to consult a Jira project's contents via a similar search function without needing to keep a Jira subscription alive. This can be useful for example when moving from Jira to another provider of project or service desk management.

# Usage
## Step 1: Obtain a Jira Cloud backup export file. 
 
 Jira provides a backup export functionality that produces a zip file which consists of:
* entities.xml
* activeobjects.xml
* data
	* attachments
	* avatars


**Extract it in its entirety. 
If this Jira export file in cloud format is not available to you, this viewer cannot be used.**

## Step 2: Run JiraCloudBackupViewer and select entitites.xml
*entities.xml* is a large 'database' file of sorts in the base folder of the export. Relevant data is read **entirely in memory**.

> **Be warned, no attempt was made to optimise for large files!**

The following data types are extracted from the XML for the purpose of searching through them and displaying them:
- `<Issue>`
- `<Action>`
- `<FileAttachment>`
- `<User>`

Search results will be shown later in a web browser. To support "downloading" attachments, the attachment path is linked to a virtual directory. 
> **It is assumed this path *data/attachments* is in the same directory as *entities.xml*, so extract all Jira files in the same place!**

The files in data/attachments are not directly viewable. Their metadata is in entities.xml in the *FileAttachment* elements. They can be viewed and 'downloaded' properly when searched through the JiraCloudBackupViewer.

## Step 3: Search loaded Issues
Search by keywords divided by space is supported. 
The default and only operator between keywords is "and".

Again, no attempt at optimization was made here: 
- The Issues list is cloned to a temporary list (references only)
- loop through keywords:
	- Filter temporary list by this keyword
	- Replace temporary list with filtered list.
	- Repeat until last keyword.
- If your keywords are any good, the resulting filtered list will be manageable.
	- If not, well..

## Step 4: Select and view search result
Search resulting issues will be shown in a table containing some useful properties from the attribbutes of `<Issue>`.

Select an issue to view it in the a web browser contained within the form. Attachments including embedded images can be downloaded using links at the top of the viewer.

Related "Action"s (such as comments) are shown in order below the main Issue summary and description.

##Step 5: Enjoy!


# Dependencies:
* Microsoft.Web.WebView2 (aka "Edge")
	* For showing Jira "markdown" in readable format
* Markdig 
	* for converting Markdown to HTML

# Techniques
* WinForms with DataGridView
	* Yes, I know and don't care.
	* `SortableBindingList` (from System.Data.Linq originally) allows for easy loading of results and sorting any column.
	* https://referencesource.microsoft.com/#system.data.linq/SortableBindingList.cs
* `System.Linq.Xml`
	* an `XDocument` Contains last loaded entities.xml file.
	* Used when selecting and loading entities.xml to immediately extract relevant data
	* Left in memory for **your** convenience and later use.	
* Classes `SearchIssue, SearchAction, SearchFileAttachment` 
	* instances hereof contain useful data from their XML counterparts, including each XElement they're based on.
	* hierarchically structured under `SearchIssue` based on issuenr.
* `Regex`. Lots of Regex.
	* Jira has its own proprietary md-like notation, which sort of matches Markdown but not quite. 
	* This notation is converted to Markdown as much as possible before rendering it using a Markdown to HTML converter.
* `CueBanner`
	* For displaying that fancy text in a search box before you enter anything.
	* It's not available readily in Winforms.
		* (Yes I know, still don't care)

# Acknowledgements
* CueBanner implementation by Tergiver:
https://social.msdn.microsoft.com/Forums/windows/en-US/e0e9191c-d4a3-4d18-91e3-5bc3d1ff7472/is-there-a-way-to-do-default-text-with-a-winforms-textbox?forum=winforms
* Jira2Md by Kyle Farris:
https://github.com/kylefarris/J2M
https://github.com/kylefarris/J2M/blob/master/index.js
