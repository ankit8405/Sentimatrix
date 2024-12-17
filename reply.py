import os
import smtplib
from email.mime.text import MIMEText
from email.mime.multipart import MIMEMultipart
from dotenv import load_dotenv
from groq import Groq

class PositiveFeedbackReplyBot:
    def __init__(self, groq_api_key, smtp_email, smtp_password):
        """
        Initialize the Positive Feedback Reply Bot
        
        Args:
            groq_api_key (str): API key for Groq
            smtp_email (str): Email address used for sending
            smtp_password (str): Email password or app-specific password
        """
        self.groq_client = Groq(api_key=groq_api_key)
        self.smtp_email = smtp_email
        self.smtp_password = smtp_password

    def generate_reply(self, original_email, sender_email=None, receiver_email=None):
        """
        Generate an automated reply using Groq API
        
        Args:
            original_email (str): The original positive feedback email
            sender_email (str, optional): Email address of the sender
            receiver_email (str, optional): Email address of the receiver
        
        Returns:
            dict: Generated reply details including message and email addresses
        """
        try:
            # Use Groq's chat completion to generate a personalized reply
            response = self.groq_client.chat.completions.create(
                model="llama-3.1-8b-instant",
                messages=[
                    {
                        "role": "system",
                        "content": """
                        You are a professional customer support representative. 
                        Generate a warm, appreciative, and personalized reply 
                        to a positive customer feedback email. 
                        The reply should:
                        - Express genuine gratitude
                        - Reflect the specific points in the original email
                        - Maintain a friendly and professional tone
                        - Encourage continued engagement
                        """
                    },
                    {
                        "role": "user",
                        "content": f"Original Positive Feedback Email:\n{original_email}\n\nPlease generate a thoughtful reply."
                    }
                ],
                max_tokens=300  # Limit response length
            )
            
            # Extract the generated reply
            reply_text = response.choices[0].message.content.strip()
            
            return {
                "reply": reply_text,
                "sender_email": self.smtp_email, 
                "receiver_email": sender_email  # Sending back to the original sender
            }
        
        except Exception as e:
            print(f"Error generating reply with Groq: {e}")
            # Fallback generic reply
            return {
                "reply": """
                Thank you so much for your wonderful feedback! 
                We truly appreciate hearing about your positive experience. 
                Your satisfaction is our top priority, and we're delighted we could meet your expectations.

                Best regards,
                Customer Support Team
                """,
                "sender_email": self.smtp_email,
                "receiver_email": sender_email
            }

    def send_email(self, to_email, subject, body):
        """
        Send an email using SMTP
        
        Args:
            to_email (str): Recipient's email address
            subject (str): Email subject
            body (str): Email body
        """
        try:
            # Create message container
            msg = MIMEMultipart()
            msg['From'] = self.smtp_email
            msg['To'] = to_email
            msg['Subject'] = subject

            # Attach the body of the message
            msg.attach(MIMEText(body, 'plain'))

            # Establish a secure session with Gmail's outgoing SMTP server using your gmail account
            server = smtplib.SMTP('smtp.gmail.com', 587)
            server.starttls()  # Secure the connection
            server.login(self.smtp_email, self.smtp_password)

            # Send email
            server.send_message(msg)
            server.quit()

            print(f"Email sent successfully to {to_email}")
        except Exception as e:
            print(f"Failed to send email to {to_email}. Error: {e}")

    def process_positive_feedback(self, positive_feedback_items):
        """
        Process list of positive feedback emails and send replies
        
        Args:
            positive_feedback_items (list): List of dictionaries containing email details
        
        Returns:
            list: Generated replies for each email
        """
        replies = []
        
        for item in positive_feedback_items:
            try:
                # Extract details
                email_text = item.get('email', '')
                sender_email = item.get('sender_email')
                
                # Generate personalized reply
                reply = self.generate_reply(
                    original_email=email_text, 
                    sender_email=sender_email
                )
                
                # Send the email
                if sender_email:
                    self.send_email(
                        to_email=sender_email,
                        subject="Thank You for Your Feedback",
                        body=reply['reply']
                    )
                
                replies.append(reply)
                print("Processed and sent reply for positive feedback email")
            
            except Exception as e:
                print(f"Error processing email: {e}")
                replies.append(None)
        
        return replies

def main():
    # Load environment variables
    load_dotenv()
    
    # Retrieve credentials from environment variables
    groq_api_key = os.getenv('GROQ_API_KEY')
    smtp_email = os.getenv('SMTP_EMAIL')
    smtp_password = os.getenv('SMTP_PASSWORD')
    
    # Specific email to send all feedback to
    specific_feedback_email = os.getenv('FEEDBACK_COLLECTION_EMAIL', 'datamaticssentimatrix@gmail.com')
    
    # Validate credentials
    if not all([groq_api_key, smtp_email, smtp_password]):
        raise ValueError("Please set GROQ_API_KEY, SMTP_EMAIL, and SMTP_PASSWORD in .env file")
    
    # Initialize the bot
    bot = PositiveFeedbackReplyBot(
        groq_api_key=groq_api_key,
        smtp_email=smtp_email,
        smtp_password=smtp_password
    )
    
    # Example positive feedback
    positive_feedback = "I absolutely loved the service! The team was incredibly helpful and went above and beyond my expectations."
    
    try:
        # Generate reply using Groq
        reply = bot.generate_reply(
            original_email=positive_feedback, 
            sender_email=specific_feedback_email
        )
        
        # Send the feedback to the specific collection email
        bot.send_email(
            to_email=specific_feedback_email,
            subject="Positive Customer Feedback",
            body=f"Feedback:\n{positive_feedback}\n\nGenerated Reply:\n{reply['reply']}"
        )
        
        print(f"Processed and sent feedback to {specific_feedback_email}")
    
    except Exception as e:
        print(f"Error processing feedback: {e}")

if __name__ == "__main__":
    main()