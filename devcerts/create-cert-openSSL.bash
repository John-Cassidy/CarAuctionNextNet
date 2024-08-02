# Create Cert with OpenSSL
openssl req -x509 -nodes -days 365 -newkey rsa:2048 -keyout carauctionnext.com.key -out carauctionnext.com.crt -config carauctionnext.com.conf

# Create pfx file
openssl pkcs12 -export -out carauctionnext.com.pfx -inkey carauctionnext.com.key -in carauctionnext.com.crt

