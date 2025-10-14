# User Guide

This document describes how end users interact with the Product and Category Management System through the web interface.

## Overview

The Product and Category Management System is a web application that allows users to:
- Manage product catalog with categories
- Perform CRUD operations (Create, Read, Update, Delete) on products and categories
- Search, filter, and sort products
- View paginated product lists
- Generate grouped reports showing product statistics by category

## Access and Navigation

The application is accessed via browser at `http://localhost:[port]/` (default port configured in IIS Express or Kestrel).

### Main Navigation Sections

- **Products**: Main product listing with search, filter, and pagination
- **Categories**: Category management interface
- **Grouped Report**: Statistical report showing products grouped by category
- **About**: Application information

## Functionalities

### Product Management

#### View Products
- Navigate to **Products** from the main menu
- View paginated list of products (10 items per page by default)
- Each product displays: Name, Price, Category, and Creation Date
- Use pagination controls at the bottom to navigate through pages

#### Search and Filter Products
- **Search by name**: Enter text in the search box to filter products by name
- **Filter by category**: Select a category from the dropdown to show only products in that category
- **Sort**: Choose sort field (Name, Price, Category) and direction (Ascending/Descending)
- Click **Filter** to apply filters or **Clear** to reset

#### Create New Product
1. Click **Create New Product** button
2. Fill in the form:
   - **Product Name** (required, 2-200 characters)
   - **Price** (required, must be greater than 0)
   - **Category** (required, select from dropdown)
3. Click **Create** to save or **Cancel** to return

#### Edit Product
1. Click **Edit** button next to the product
2. Modify the fields as needed
3. Click **Save** to update or **Cancel** to discard changes

#### Delete Product
1. Click **Delete** button next to the product
2. Review the product details in the confirmation page
3. Click **Delete** to confirm or **Cancel** to abort

#### View Product Details
- Click **Details** button to view complete product information including creation and update timestamps

### Category Management

#### View Categories
- Navigate to **Categories** from the main menu
- View complete list of all categories
- Each category displays: Name, Creation Date, and Last Update Date

#### Create New Category
1. Click **Create New Category** button
2. Enter **Category Name** (required, 2-100 characters)
3. Click **Create** to save or **Cancel** to return

#### Edit Category
1. Click **Edit** button next to the category
2. Modify the category name
3. Click **Save** to update or **Cancel** to discard changes

#### Delete Category
1. Click **Delete** button next to the category
2. **Warning**: Deleting a category will also delete all associated products (CASCADE DELETE)
3. Click **Delete** to confirm or **Cancel** to abort

### Grouped Report

Navigate to **Grouped Report** to view product statistics by category:
- **Category Name**
- **Product Count**: Number of products in the category
- **Total Value**: Sum of all product prices in the category
- **Average Price**: Average price of products in the category
- **Min Price**: Lowest priced product in the category
- **Max Price**: Highest priced product in the category

The report includes a total row showing overall product count and total value.

## Interface Behavior

### Validation
- **Client-side validation**: Forms validate input before submission using jQuery validation
- **Server-side validation**: API validates all data and returns detailed error messages
- **Required fields**: Marked with validation messages if left empty
- **Format validation**: Price must be numeric and positive, names must meet length requirements

### Feedback Messages
- **Success alerts** (green): Displayed after successful create, update, or delete operations
- **Error alerts** (red): Displayed when operations fail or validation errors occur
- **Warning alerts** (yellow): Displayed for important notices (e.g., cascade delete warning)

### Responsive Design
- Interface uses Bootstrap for responsive layout
- Works on desktop, tablet, and mobile devices
- Navigation collapses to hamburger menu on small screens

## Limitations

- **No authentication**: Application does not implement user authentication or authorization
- **Single language**: Interface is in Spanish/English mixed (can be localized)
- **No file uploads**: Products do not support images or attachments
- **Local database**: Uses SQLite database stored locally (not suitable for production multi-user scenarios)
- **No real-time updates**: Changes made by other users require page refresh to see
- **Basic search**: Search only works on product name (not description or other fields)
