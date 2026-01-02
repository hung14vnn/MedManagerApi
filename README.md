# MedManager API

A comprehensive .NET 10 backend API for pharmacy medication management using PostgreSQL.

## Features

### 1. Drug Information Lookup
- Search by: Active ingredient name, brand name, and pharmacological group
- Comprehensive drug information including:
  - Indications & Contraindications
  - Dosage (Adults, children, patients with hepatic/renal impairment)
  - Adverse effects
  - Special precautions for pregnancy and breastfeeding

### 2. Drug Interaction Checker
- Check interactions between multiple drugs simultaneously
- Interaction severity classification: Mild, Moderate, Severe
- Detailed explanation of interaction mechanisms
- Management recommendations
- Referenced clinical literature

### 3. Disease-Based Lookup
- Standard treatment protocol suggestions
- Preferred drugs vs Alternative drugs
- Dose adjustments based on underlying conditions

### 4. Clinical Pharmacy Support Tools
- Dose calculation based on body weight, creatinine clearance, eGFR
- Patient medication counseling checklist

## Technology Stack

- **.NET 10** - Latest .NET framework
- **Entity Framework Core 9.0** - ORM for database operations
- **PostgreSQL** - Relational database
- **Npgsql** - PostgreSQL provider for EF Core

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [PostgreSQL 12+](https://www.postgresql.org/download/)
- Your favorite IDE (Visual Studio 2025, VS Code, or Rider)

## Setup Instructions

### 1. Clone the Repository
```bash
cd MedManagerApi
```

### 2. Configure Database Connection

Update the connection string in `appsettings.Development.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=medmanager;Username=your_username;Password=your_password"
  }
}
```

### 3. Install EF Core Tools (if not already installed)

```bash
dotnet tool install --global dotnet-ef
```

### 4. Create Database Migration

```bash
dotnet ef migrations add InitialCreate
```

### 5. Apply Migration to Database

```bash
dotnet ef database update
```

### 6. Run the Application

```bash
dotnet run
```

The API will be available at:
- HTTP: `http://localhost:5000`
- HTTPS: `https://localhost:5001`
- OpenAPI/Swagger: `https://localhost:5001/openapi/v1.json`

## API Endpoints

### Drugs

- **GET** `/api/drugs?search={term}` - Search for drugs
- **GET** `/api/drugs/{id}` - Get drug details by ID
- **POST** `/api/drugs` - Create a new drug
- **PUT** `/api/drugs/{id}` - Update drug information
- **DELETE** `/api/drugs/{id}` - Delete a drug
- **POST** `/api/drugs/{id}/references` - Add reference to a drug

### Interactions

- **POST** `/api/interactions/check` - Check interactions between drugs
  ```json
  {
    "drugIds": [1, 2, 3]
  }
  ```
- **GET** `/api/interactions/{id}` - Get interaction details
- **POST** `/api/interactions` - Create new interaction record
- **DELETE** `/api/interactions/{id}` - Delete interaction
- **POST** `/api/interactions/{id}/references` - Add reference to interaction

### Diseases

- **GET** `/api/diseases` - Get all diseases
- **GET** `/api/diseases/{id}` - Get disease details
- **GET** `/api/diseases/{id}/treatment` - Get treatment protocol for disease
- **POST** `/api/diseases` - Create new disease
- **POST** `/api/diseases/protocols` - Add treatment protocol

## Database Schema

### Main Tables

1. **Drugs** - Drug information
2. **DrugInteractions** - Drug-to-drug interactions
3. **DrugReferences** - Scientific references for drugs
4. **InteractionReferences** - References for interactions
5. **Diseases** - Disease catalog
6. **DiseaseProtocols** - Treatment protocols
7. **DoseCalculations** - Dosing formulas
8. **CounselingChecklists** - Patient counseling points

## Sample API Requests

### Create a Drug

```bash
POST /api/drugs
Content-Type: application/json

{
  "activeIngredient": "Amoxicillin",
  "brandName": "Amoxil",
  "pharmacologicalGroup": "Beta-lactam Antibiotics",
  "indications": "Bacterial infections",
  "contraindications": "Penicillin allergy",
  "dosageAdults": "500mg three times daily",
  "dosageChildren": "25-45 mg/kg/day divided into 2-3 doses",
  "adverseEffects": "Diarrhea, nausea, skin rash",
  "pregnancyPrecautions": "Category B - Generally safe",
  "breastfeedingPrecautions": "Safe in breastfeeding"
}
```

### Check Drug Interactions

```bash
POST /api/interactions/check
Content-Type: application/json

{
  "drugIds": [1, 2, 5]
}
```

### Get Treatment Protocol

```bash
GET /api/diseases/1/treatment
```

## Development Notes

- The API uses Entity Framework Core with PostgreSQL
- All timestamps are stored in UTC
- API returns JSON responses
- CORS is enabled for all origins in development

## Future Enhancements

- [ ] Add authentication and authorization
- [ ] Implement caching for frequently accessed data
- [ ] Add full-text search capabilities
- [ ] Create batch import functionality
- [ ] Add API versioning
- [ ] Implement logging and monitoring
- [ ] Add unit and integration tests

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

## License

This project is licensed under the MIT License.
