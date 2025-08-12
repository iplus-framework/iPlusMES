# iPlus-MES

iPlus-MES is a highly configurable, extendable and adaptable Manufacturing Execution System (MES) designed to bridge the gap between enterprise-level systems (ERP) and field-level operations. Built on the iPlus-framework, it offers a robust solution for modern industrial challenges, ensuring optimal stability, performance, and flexibility.
![myimage-alt-tag](https://github.com/iplus-framework/iPlus-Docs/blob/main/Images/iplus-MES-Screenshot.jpg)

## Features

- **Stability and Performance**: Ensures optimal operation and user-friendly presentation of complex production processes.
- **Architecture**: Utilizes a normalized database model that adapts to existing production and IT systems.
- **Industry Agnostic**: Suitable for various sectors due to its customizable structure. Focused on process industry.
- **DCS**: Distributed control system
- **Usability**: Features an intuitive graphical user interface, merging SCADA and MES functionalities.
- **Comprehensive Modules**:
  - Master Data Management
  - Production and Control
  - Material Management
  - Logistics
  - Quality Management
  - Maintenance
- **Real openess and extendability**:
  - Real openness because it can be expanded as required. Either by inheriting existing components. Developing additional components. Using iPlus packages that contain industry solutions...

## Why Choose iPlus-MES?
- **Resource Utilization**: Helps achieve perfect resource utilization and high efficiency.
- **Transparency**: Renders all processes considerably more transparent.
- **Proven Success**: Over 30 installations, 500+ workplaces/users, and 4000+ machines and devices.

## Technology
- Build on .NET Platform: .NET Core, Entity Framework, WCF, WPF, iplus-framework
- Serverside without UI: Windows, Linux
- Clientside with UI: Windows. On Linux/Android currently with WINE, but in the future with Avalonia XPF (see library exception in our GPL-v3 license) also on Linux, Android, macOS, iOS and Web-Browser.

## Getting Started
To get started with the iPlus-MES, follow these steps:

1. **Clone the Repository:**
  - You need [iplus-framework](https://github.com/iplus-framework/iPlus) first. Read and clone it.
  - Then: git clone https://github.com/iplus-framework/iPlusMES.git
  - Make sure to clone this repository into the same root path as iplus-framework. (So that our repositories are all in a common directory, so that the specified relative paths match.)

3. **Install a Microsoft SQL-Server Edition:**  
https://learn.microsoft.com/en-us/sql/database-engine/install-windows/install-sql-server?view=sql-server-ver16

4. **Restore a MES-database file:**  
Restore the SQL Server backup file located in the "Database" folder. Use the IPCDrivesV5-database for playing and exploring features. Use iplusMESV5 for starting building you own MES from scratch.

5. **Open project in Microsoft Visual Studio:**
  - Modify the ConnectionStrings.config file, Follow the instructions on [gipSoft's documentation site](https://iplus-framework.com/en/documentation/Read/Index/View/b00675a8-718c-4c13-9d6d-5e751397ac5f?chapterID=193d292e-df31-405e-a3e9-f1116846bf86#ItemTextTranslationID_d0551cc7-f767-4790-8ecb-8771836ebac7)
  - Compile the solution an set gip.mes.client.exe as start project.

6. **Execute gip.mes.client.exe:**
  - Login with user "superuser" and password "superuser" to **start as a client**. Login with user "00" and password "00" to **start as a service**. After starting the service, your local IP is remembered and you can start a second instance an logon as client to establish the Client-Server-Connection. If you have problems, that the Client doesn't connect with the server read the [network configuration section](https://iplus-framework.com/en/documentation/Read/Index/View/b00675a8-718c-4c13-9d6d-5e751397ac5f?chapterID=a7424f03-cbff-4f68-90eb-262b3fdcba1c#ItemTextTranslationID_bf788469-8a2f-4a7c-ad78-2710960096dc). For [running on Linux read here](https://iplus-framework.com/en/documentation/Read/Index/View/b00675a8-718c-4c13-9d6d-5e751397ac5f?chapterID=9df14383-a4e5-4a3d-81d5-5dccf80d6c4e).
  - *Important: Every time you pull a newer version from GitHub, you should **hold down the CTRL key** while clicking on the [login button in the login window](https://iplus-framework.com/en/documentation/Read/Index/View/b00675a8-718c-4c13-9d6d-5e751397ac5f?chapterID=ca1a96f0-233d-4e6c-9cfc-d1db683e4be3#ItemTextTranslationID_e24a12c4-649d-4903-abf0-8366ed656ca1) so that your local databases are updated.* Otherwise, the application will not be able to start.
  - 🤗 Have fun trying it out and watch our videos to get started. However, we recommend [online training](https://iplus-framework.com/en?section=Support%20%26%20Training#d57ccb45-9050-41cb-a177-9e8c05028931).

## Contributing

We welcome contributions from the community! 

Are you passionate about open-source development and eager to make a real impact in the world of industrial software? The gipSoft is looking for talented developers like you to help us push the boundaries of innovation and autonomy in software solutions.

By contributing to the iPlus-framework, you'll be part of a dynamic community dedicated to creating flexible, scalable, and efficient software that empowers businesses to thrive in the era of Industry 4.0. Your expertise and creativity will directly influence the evolution of our platform, helping to solve complex technical challenges and drive the future of manufacturing technology.

**Why Contribute?**

**Make an Impact:** Your contributions will help shape a powerful tool that supports businesses worldwide in achieving greater autonomy and efficiency.
Collaborate and Innovate: Work alongside a passionate community of developers and industry experts, sharing knowledge and driving innovation.

**Revenue Sharing:** As a token of our appreciation, contributors will receive a share of the revenue generated from the commercial edition of the iPlus-MES, recognizing your valuable input and dedication.
Join us on GitHub and be part of a project that values collaboration, innovation, and shared success. Together, we can build a more sustainable and independent future for industrial software.

Contribute today and let's create something amazing together!
More about our [rewarding model](https://iplus-framework.com/en/documentation/Read/Index/View/a4100937-4d88-487d-ab3b-e599412e2a2f?workspaceSchemaID=ab0bc53f-decf-4101-9cee-111b6cbc5b24).

## Limitations
We have a [fork of entity framework core](https://github.com/iplus-framework/efcore/tree/ef_for_iPlus_compilable) that fixes the following issues:
1. Ability to refresh a context from the database
https://github.com/dotnet/efcore/issues/16491  
This point is problematic if you are working with long-term database contexts and already loaded entity objects and relationships to other objects have to be reloaded from the database. An example would be if you change a workflow on the client side that was already in use on the server side and it has to be reloaded.

2. DetectChanges slow in long term contexts
https://github.com/dotnet/efcore/issues/31819  
This point is problematic when you work with long-term database contexts and larger amounts of data, because the system then becomes slow.

3. Reverting changes
https://github.com/dotnet/efcore/issues/14594

If you want to use **iPlus for productive operation** use this release 9.0 fork instead of the official nuget packages. Open the [Directory.Build.props](https://github.com/iplus-framework/iPlus/blob/main/Directory.Build.props) file and set **UseEFCoreForkIPlus to True to** include the compiled ef-assemblies.

## Documentation

For now, please use the [documentation on our website]([https://iplus-framework.com/en/documentation/Home/Schema/View/bce1702a-7637-4b98-83db-01a9d7a3a156](https://iplus-framework.com/en/documentation/Home/Schema/View/e0703b3b-647a-46a6-8c53-8706e9cb128b)).

## License

The iPlus-MES is licensed under the GPL-V3 License. See the [LICENSE](LICENSE) file for more details.

The commercial edition is intended for end customers for commercial use. See this [LICENSE](https://iplus-framework.com/en/documentation/Read/Index/View/8dec2941-f7ed-4bed-92a5-0e07404e359a?workspaceSchemaID=ab0bc53f-decf-4101-9cee-111b6cbc5b24) file for more details.

## Contact

For questions, suggestions, or feedback, please send us a e-mail to **info[at]iplus-framework.com** or [via contact form](https://iplus-framework.com/en?section=Contact#aedf447c-2a3a-4bfe-9e0a-a9c5740e4f8e).

We look forward to your contributions and hope you enjoy working with the iPlus-MES!
