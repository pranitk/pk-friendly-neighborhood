import pyodbc

#Connection string
connection_string = "CONNECTION_STRING"

# Connect to Azure SQL Database using connection string
def connect_to_database():
    try:
        conn = pyodbc.connect(connection_string)
        print("Connected to Azure SQL Database successfully")
        return conn
    except Exception as e:
        print("Error connecting to database:", e)
        return None

#CREATE TABLE TestData (
#     PatientID UNIQUEIDENTIFIER PRIMARY KEY,
#     Relationship NVARCHAR(50),
#     ZipCode NVARCHAR(10),
#     Age INT,
#     Gender NVARCHAR(10),
#     Diagnosis NVARCHAR(1000),
#     LanguagePreference NVARCHAR(50),
#     Summary NVARCHAR(MAX),
#     ContactPhoneNumber NVARCHAR(15),
#     Email NVARCHAR(100)
# );


# Function to insert test data into database
def insert_test_data(data):
    conn = connect_to_database()
    if conn is None:
        return False, "Failed to connect to database"

    try:
        cursor = conn.cursor()
        for record in data:
            # Extract data from JSON
            patient_id = record.get('PatientID')
            relationship = record.get('Relationship')
            zip_code = record.get('ZipCode')
            age = record.get('Age')
            gender = record.get('Gender')
            diagnosis = record.get('Diagnosis')
            language_preference = record.get('LanguagePreference')
            summary = record.get('Summary')
            contact_phone_number = record.get('ContactPhoneNumber')
            email = record.get('Email')

            # Insert record into database
            cursor.execute("INSERT INTO TestData (PatientID, Relationship, ZipCode, Age, Gender, Diagnosis, LanguagePreference, Summary, ContactPhoneNumber, Email) VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?)", (patient_id, relationship, zip_code, age, gender, diagnosis, language_preference, summary, contact_phone_number, email))
        conn.commit()
        return True, "Test data inserted successfully"
    except Exception as e:
        return False, str(e)
    finally:
        conn.close()

# Function to read records from database
def read_records_from_database():
    conn = connect_to_database()
    if conn is None:
        return []

    try:
        cursor = conn.cursor()
        cursor.execute("SELECT * FROM TestData")
        records = cursor.fetchall()
        return records
    except Exception as e:
        print("Error reading records from database:", e)
        return []
    finally:
        conn.close()