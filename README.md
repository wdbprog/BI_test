# BI_test

## Running the Backend (ASP.NET Core API)

1. Open a terminal in the `bi_api` folder.
2. The default URL is `https://localhost:8080`, if you want to change it set it in `appsettings.json`
3. Run the backend API:
   ```cmd
   dotnet run
   ```
4. The API will be available at the URL specified in `appsettings.json` (e.g., `https://localhost:8080`).

## Running the Frontend (React + TypeS0cript)

1. Open a terminal in the `bi_frontend` folder.
2. Make sure the backend API URL is set in the `.env` file:
   ```env
   REACT_APP_API_URL=https://localhost:8080
   ```
   This should match the backend URL from `appsettings.json`.
3. Install dependencies:
   ```powershell
   npm install
   ```
4. Start the frontend app:
   ```powershell
   npm start
   ```
5. The frontend will run on `http://localhost:3000` by default

## Notes
- If you change the backend or frontend URLs/Ports, check both `.env` (frontend) and `appsettings.json` (backend).