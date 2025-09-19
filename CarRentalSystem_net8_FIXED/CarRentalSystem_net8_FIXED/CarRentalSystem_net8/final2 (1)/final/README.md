# Car Rental Management System

A comprehensive web-based car rental management system built with ASP.NET Core MVC, Entity Framework Core, and SQLite database.

## Features

### Admin Features
- **Dashboard**: Overview of system operations
- **Car Management**: Add, edit, delete, and view all cars in the system
- **Booking Management**: View and manage all customer bookings
- **User Management**: System administration capabilities

### Customer Features
- **Car Browsing**: View available cars with images and details
- **Booking System**: Book cars with date selection and cost calculation
- **Booking Management**: View, track, and cancel personal bookings
- **User Registration**: Create customer accounts

### System Features
- **Session-based Authentication**: Secure login/logout functionality
- **Role-based Access Control**: Separate interfaces for Admin and Customer users
- **Responsive Design**: Bootstrap 5 UI that works on all devices
- **Real-time Cost Calculation**: Automatic pricing based on rental duration
- **Booking Validation**: Prevents double bookings and invalid date ranges
- **Database Integration**: SQLite database with Entity Framework Core

## Technology Stack

- **Backend**: ASP.NET Core 7.0 MVC
- **Database**: SQLite with Entity Framework Core
- **Frontend**: Bootstrap 5, HTML5, CSS3, JavaScript
- **Authentication**: Session-based authentication
- **Icons**: Bootstrap Icons

## Getting Started

### Prerequisites
- .NET 7.0 SDK or later
- Visual Studio 2022 or VS Code (optional)

### Installation

1. **Clone or download the project**
   ```bash
   cd CarRentalSystem
   ```

2. **Restore packages**
   ```bash
   dotnet restore
   ```

3. **Create and update the database**
   ```bash
   dotnet ef database update
   ```

4. **Run the application**
   ```bash
   dotnet run
   ```

5. **Access the application**
   - Open your browser and navigate to `https://localhost:5001` or `http://localhost:5000`
   - The application will automatically redirect you to the login page

### Default Login Credentials

**Admin Account:**
- Username: `admin`
- Password: `admin123`
- Role: Admin

**Customer Account:**
- You can register a new customer account using the registration form

## Database Schema

### Users Table
- `UserID` (Primary Key)
- `Username` (Unique)
- `Password`
- `Role` (Admin/Customer)

### Cars Table
- `CarID` (Primary Key)
- `CarName`
- `CarModel`
- `ImageUrl`
- `IsAvailable` (Boolean)

### Bookings Table
- `BookingID` (Primary Key)
- `CustomerID` (Foreign Key to Users)
- `CarID` (Foreign Key to Cars)
- `PickupDate`
- `ReturnDate`
- `TotalCost`

## Project Structure

```
CarRentalSystem/
├── Controllers/
│   ├── AccountController.cs      # Authentication (Login/Register/Logout)
│   ├── AdminController.cs        # Admin functionality
│   ├── CustomerController.cs     # Customer functionality
│   └── HomeController.cs         # Home page routing
├── Models/
│   ├── User.cs                   # User entity
│   ├── Car.cs                    # Car entity
│   ├── Booking.cs                # Booking entity
│   └── ViewModels/
│       ├── LoginViewModel.cs     # Login form model
│       ├── RegisterViewModel.cs  # Registration form model
│       └── BookingViewModel.cs   # Booking form model
├── Views/
│   ├── Account/                  # Authentication views
│   ├── Admin/                    # Admin interface views
│   ├── Customer/                 # Customer interface views
│   └── Shared/                   # Shared layouts and partials
├── Data/
│   └── ApplicationDbContext.cs   # Entity Framework context
└── wwwroot/                      # Static files (CSS, JS, images)
```

## Usage Guide

### For Administrators

1. **Login** with admin credentials
2. **Manage Cars**:
   - Add new cars to the inventory
   - Edit existing car information
   - Delete cars from the system
   - View all cars and their availability status

3. **Manage Bookings**:
   - View all customer bookings
   - Delete bookings if necessary
   - Monitor booking patterns

### For Customers

1. **Register** a new account or **Login** with existing credentials
2. **Browse Cars**:
   - View available cars with images and details
   - See rental rates and availability status

3. **Book a Car**:
   - Select pickup and return dates
   - View calculated total cost
   - Confirm the booking

4. **Manage Bookings**:
   - View all personal bookings
   - Track booking status (Upcoming/Active/Completed)
   - Cancel upcoming bookings

## Business Logic

### Pricing
- Standard rate: $50 per day
- Total cost = Number of days × $50
- Minimum rental period: 1 day

### Booking Rules
- Pickup date cannot be in the past
- Return date must be after pickup date
- Cars cannot be double-booked for overlapping dates
- Customers can only cancel upcoming bookings

### Availability
- Cars are marked as available/unavailable
- System prevents booking of unavailable cars
- Booking conflicts are automatically detected

## Development Notes

### Database Migrations
To create a new migration after model changes:
```bash
dotnet ef migrations add MigrationName
dotnet ef database update
```

### Adding New Features
1. Update models in the `Models/` folder
2. Create/update controllers in the `Controllers/` folder
3. Create corresponding views in the `Views/` folder
4. Update the database context if needed
5. Create and apply migrations

## Security Features

- Session-based authentication
- Role-based authorization
- CSRF protection on forms
- Input validation and sanitization
- SQL injection prevention through Entity Framework

## Future Enhancements

- Payment integration
- Email notifications
- Advanced search and filtering
- Car categories and pricing tiers
- Customer reviews and ratings
- Admin reporting dashboard
- Mobile app integration
