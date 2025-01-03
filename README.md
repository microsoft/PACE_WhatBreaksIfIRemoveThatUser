# Flow Ownership Audit Tool
Developed as part of the CSU Pace Setters Program

## What it does
Flow Ownership Audit Tool is an XrmToolBox plugin designed for administrators and power users. 
It enables the efficient tracking and management of components owned by individual users. 
This tool is particularly useful for overseeing transitions when a user departs from the company, ensuring continuity and control over user-specific workflows and connections. 
With its user-friendly interface, Flow and Connect Auditor simplifies the process of fetching and auditing flow data, making it an indispensable asset for your administrative toolkit.

## How it works
The Flow and Connect Auditor plugin efficiently retrieves data regarding user-owned components. 
As XrmToolBox inherently lacks these specific scopes, the plugin incorporates a bespoke authentication mechanism. 
This dedicated approach ensures accurate and secure access to the necessary information, bypassing the limitations of standard authentication profiles. 
It's a tailored solution for comprehensive flow data management within the XrmToolBox environment.

## How to use the tool
As previously mentioned, the standard authentication profiles provided by XrmToolBox cannot be utilized for this plugin.

Once the tool is active, you will be guided through the following steps to fetch the desired flow information:

- Choose the environments to query, which will prompt an authentication window to sign in with the appropriate scopes.
- Input the email address of the user whose flow information you wish to retrieve.
- Specify the type of information required: Flows, Connections, Connection References.
- Click the start button to begin the information retrieval process.
- Post-query, you have the option to export the gathered data to an Excel document for further analysis.
- Post-query, you have the ability to reassign flows to a different user.

Please note, the duration for the tool to compile all information is contingent on the number of selected environments, as well as the volume of components within those environments.