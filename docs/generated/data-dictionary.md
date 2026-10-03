# Diccionario de datos

## Tabla: Flights

| Campo | Tipo | Restricciones | Descripción |
|---|---|---|---|
| Id | integer | PK, identity | Identificador del vuelo |
| FlightNumber | varchar(10) | NOT NULL | Código de vuelo |
| Airline | varchar(100) | NOT NULL | Aerolínea |
| Origin | varchar(80) | NOT NULL, index | Ciudad de origen |
| Destination | varchar(80) | NOT NULL, index | Ciudad de destino |
| DepartureTime | timestamp | NOT NULL, index | Salida UTC |
| ArrivalTime | timestamp | NOT NULL | Llegada UTC |
| Price | numeric(10,2) | NOT NULL | Precio en PEN |
| TotalSeats | integer | NOT NULL | Capacidad total |

## Tabla: Reservations

| Campo | Tipo | Restricciones | Descripción |
|---|---|---|---|
| Id | integer | PK, identity | Identificador de reserva |
| UserId | varchar(80) | NOT NULL | Identificador lógico del usuario |
| PassengerName | varchar(120) | NOT NULL | Nombre del pasajero |
| PassengerEmail | varchar(160) | NOT NULL | Correo del pasajero |
| SeatNumber | varchar(8) | NOT NULL, UNIQUE con FlightId | Asiento |
| CreatedAt | timestamp | NOT NULL | Fecha de creación |
| Status | varchar(20) | NOT NULL | Confirmed/Cancelled |
| FlightId | integer | FK Flights.Id | Vuelo reservado |
