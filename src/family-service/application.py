from flask import Flask, jsonify, request
from database import *
from embedding import *
import json

from flask_cors import CORS, cross_origin

app = Flask(__name__)
CORS(app, origins=['http://localhost:8000', '*']) #update this later

# Endpoint for connection testing
@app.route("/")
def hello():
    return "Hello World!"

# Endpoint to insert test records. Sample generated from Chat GPT stored in test-data.json
@app.route("/insert-records", methods=['POST'])
def insert_test_records():
    data = request.json.get('data', [])
    success, message = insert_test_data(data)
    if success:
        return jsonify({'message': message}), 200
    else:
        return jsonify({'error': message}), 500


# Endpoint to fetch all test data records
@app.route('/get-test-data', methods=['GET'])
def get_test_data():
    records = read_records_from_database()
    if not records:
        return jsonify({'error': 'No records found'}), 404

    # Convert records to dictionary format for JSON response
    formatted_records = [{'PatientID': str(patient_id), 'Summary': summary} for patient_id, summary in records]
    return jsonify(formatted_records), 200

@app.route('/insert-generate-and-index', methods=['POST'])
def insert_generate_and_index():
    data_from_database = read_records_from_database()
    records_with_embeddings = []
    for record in data_from_database:
        text = record.Summary  

        embedding = generate_embedding_and_metadata(text)
        record_with_embedding = {
            'PatientID': record.PatientID,
            'Relationship': record.Relationship,
            'ZipCode': record.ZipCode,
            'Age': record.Age,
            'Gender': record.Gender,
            'Diagnosis': record.Diagnosis,
            'LanguagePreference': record.LanguagePreference,
            'Summary': text,
            'ContactPhoneNumber': record.ContactPhoneNumber,
            'Email': record.Email,
            'embedding': embedding
        }
        records_with_embeddings.append(record_with_embedding)

    azure_search_client.upload_documents(documents=records_with_embeddings)
    return jsonify({'message': 'Test data inserted, embeddings generated, and indexed successfully'}), 200


@app.route('/insert-generate-and-index-json', methods=['POST'])
def insert_generate_and_index_json():
    # Open the JSON file in read mode
    with open('test-data.json', 'r') as file:
    # Load the JSON data from the file
        data = json.load(file)

    data_from_database = data['data']
    print(data_from_database)
    records_with_embeddings = []
    for record in data_from_database:
        text = record['Summary'] 

        embedding = generate_embedding_and_metadata(text)
        # Convert embedding to string representation
        embedding_str = ','.join(map(str, embedding))
        record_with_embedding = {
            'PatientID': record['PatientID'],
            'Relationship': record['Relationship'],
            'ZipCode': record['ZipCode'],
            'Age': record['Age'],
            'Gender': record['Gender'],
            'Diagnosis': record['Diagnosis'],
            'LanguagePreference': record['LanguagePreference'],
            'Summary': text,
            'ContactPhoneNumber': record['ContactPhoneNumber'],
            'Email': record['Email'],
            'embedding': embedding_str
        }
        records_with_embeddings.append(record_with_embedding)

    azure_search_client.upload_documents(documents=records_with_embeddings)
    return jsonify({'message': 'Test data inserted, embeddings generated, and indexed successfully'}), 200


@app.route('/search-similar', methods=['POST'])
def search_similar():
    query_text = request.json.get('query_text', '')
    print(query_text)
    results = []
    response = search_similar_text(query_text)
    for result in response:
        results.append(result)
    
    return jsonify(results), 200

if __name__ == '__main__':
    app.run(debug=True)
