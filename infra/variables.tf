variable "app_name" { type = string; default = "flightreserve" }
variable "location" { type = string; default = "eastus2" }
variable "db_admin_username" { type = string; default = "flightadmin" }
variable "db_admin_password" { type = string; sensitive = true }
