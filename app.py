from flask import Flask, render_template, request, jsonify
import requests

app = Flask(__name__)

# Be sure to replace with your actual Google API key and Custom Search Engine ID
GOOGLE_API_KEY = 'AIzaSyDttoppLRB7DITEWfoqm1vzphTvYy-mE2Q'
GOOGLE_CX = 'd7b987669bda24906'

@app.route("/")
def home():
    return render_template("index1.html")

@app.route("/ask", methods=["POST"])
def ask():
    user_location = request.json['location']

    # Construct the query for Google Custom Search API
    query = f"autism care services near {user_location}"

    try:
        # Make the GET request to Google Custom Search API
        response = requests.get(
            'https://www.googleapis.com/customsearch/v1',
            params={
                'key': GOOGLE_API_KEY,
                'cx': GOOGLE_CX,
                'q': query,
            }
        )
        response.raise_for_status()  # Raises an HTTPError if the HTTP request returned an unsuccessful status code
        search_results = response.json()
        if 'error' in search_results:
            raise Exception(search_results['error']['message'])


        # Extract the search results and prepare a response
        if 'items' in search_results:
            resources = [
                f"{item['title']} - {item['link']}" for item in search_results['items']
            ]
            response_message = '\n'.join(resources)
        else:
            response_message = "No results found for your query."
        
    except requests.exceptions.RequestException as e:
        response_message = f"An error occurred while accessing Google Search API: {str(e)}"

    return jsonify({"message": response_message})

if __name__ == "__main__":
    app.run(debug=True)
