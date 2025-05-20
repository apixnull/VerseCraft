# VerseCraft

VerseCraft is a creative platform where users can write, upload, and share their original poems and anthologies. The app supports community interaction through exploration features, and it includes an admin panel for content moderation and platform management.

Users can create individual poems, group them into anthologies (collections), and share them with the public. Other users can view these works, like or bookmark them, and browse through collections by title, author, or description.

The application also includes a secure user authentication system and OTP (One-Time Password) verification via email using SendGrid. Two types of users exist: normal users and admin users. Admins have the ability to approve or reject poems and anthologies, manage users, and maintain the quality of published content.

# Deployed at Render
link: https://versecraft.onrender.com/ 

## Features

**For Normal Users**
- Register and log in with email and password
- OTP verification during signup and password reset
- Create, edit, and delete poems
- Create and manage anthologies (collections of poems)
- Explore other users’ published anthologies and poems
- Bookmark and like favorite works

**For Admins**
- Access the admin dashboard
- Approve or disapprove submitted poems and anthologies
- View and manage all users and their content
- Enforce content standards and remove inappropriate submissions

## Test Accounts

Admin user  
Email: jaspercesa20@gmail.com  
Password: @Apix12345

Normal user  
Email: apixnull@gmail.com  
Password: @Apix12345

## Technologies Used

- ASP.NET Core 8 (MVC Framework)
- Entity Framework Core 8 (with SQLite)
- SendGrid for email delivery
- BCrypt.Net for password hashing
- DotNetEnv for environment variable handling

## How to Run the Application

1. Install .NET 8 SDK
2. Clone or download this repository
3. Create a file named `.env` in the project root directory  
   Inside the file, add your SendGrid API key like this:  
   SENDGRID_API_KEY=your_sendgrid_api_key_here
4. Open `appsettings.json` and set your connection string and email sender settings like this:  
   - ConnectionStrings:DefaultConnection = "Data Source=VerseCraft.db"  
   - EmailSettings:FromEmail = "apixnull@gmail.com"  
   - EmailSettings:FromName = "VerseCraft"
5. Restore NuGet packages
6. Apply Entity Framework Core migrations
7. Run the application
8. Open your browser and go to `http://localhost:5000` or `https://localhost:5001`

## Environment File Sample

SENDGRID_API_KEY=your_sendgrid_api_key_here

## Project Structure

- Areas/admin/ → Admin panel pages and data
- Helpers/ → Utility classes such as email sender
- Views/ExploreAnthology/ → Public-facing anthology interface
- wwwroot/uploads/ → Image uploads folder

## License

This project is licensed under the MIT License. You are free to use, modify, and distribute this code for personal or commercial use. See the LICENSE file for full details.
