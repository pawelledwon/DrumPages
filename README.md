# DrumPages Platform

## Overview

This repository contains the source code for the DrumPages platform, a web application designed to facilitate drum learning and communication within the drumming community. The platform is built using ASP.NET Core Razor Pages and MySQL database.

## Table of Contents

1. [Description](#description)
2. [Features](#features)
3. [Installation](#installation)
4. [Usage](#usage)
5. [Contributing](#contributing)

## Description

The DrumPages platform aims to address several key issues faced by drum enthusiasts, including the dispersion of learning materials, lack of quality control, and difficulty in tracking learning progress. The platform provides users with easy access to learning materials, communication with experts, answers to complex questions, information about drum-related events, various courses, and the ability to join an online community of drum enthusiasts.

## Features

- **User Roles**: The platform supports two user roles - "Amateur" and "Pro". Users can register with basic information and choose their role.
  
- **Guest Access**: Guests can browse forum posts, access basic lessons from highly rated courses, and view upcoming events. However, they cannot post on the forum or reply to posts.

- **Amateur and Pro Users**: Logged-in users have access to more features, including watching lessons, participating in the forum, rating lessons, tracking learning progress, and receiving notifications for new content.

- **Lesson Creation and Rating**: Pro users can create and publish their own lessons, which can be rated by other users. Each lesson consists of chapters with attached files such as sheet music or recordings.

- **Event Creation**: Pro users can create drum-related events like concerts or workshops, specifying the target audience and providing relevant links.

- **Admin Panel**: Administrators have access to a panel for verifying Pro users, managing user reports, and monitoring site activity.

## Installation

To run the DrumPages platform locally, follow these steps:

1. Clone the repository:\
   git clone https://github.com/pawelledwon/DrumPages.git

2. Install dependencies:\
   dotnet restore
   
3. Set up MySQL database with attached script.

4. Access the application in your browser at localhost

## Usage

- Register an account and choose your user type (Amateur or Pro).
- Browse available lessons and courses, watch lessons, and track your progress.
- Engage with the drumming community by participating in forums and events.
- Pro users can create their own lessons and drum-related events.

## Contributing

Contributions to the DrumPages platform are welcome!
