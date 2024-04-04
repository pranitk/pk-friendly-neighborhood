from flask import Flask, render_template, request, jsonify

app = Flask(__name__)

@app.route("/")
def home():
    return render_template("index.html")

@app.route("/ask", methods=["POST"])
def ask():
    user_message = request.json['message']
    # Placeholder for processing and responding to the message
    # This is where you can integrate with a chatbot API
    response_message = f"Echo: {user_message}"
    return jsonify({"message": response_message})

if __name__ == "__main__":
    app.run(debug=True)


