# create a docker volume for our MongoDb image
docker volume create --name=mongodata

# mount and start start our MongoDb image
docker run --name mongodb -v mongodata:/data/db -d -p 27017:27017 mongo
#docker run -d --rm mongo -p 27017:27017 -v mongodbdata:/data/db mongo

# open firewall for MongoDB port
netsh advfirewall firewall add rule name="Open mongod port 27017" dir=in action=allow protocol=TCP localport=27017

# open firewall for MongoDb exe
netsh advfirewall firewall add rule name="Allowing mongod" dir=in action=allow program=" C:\Program Files\MongoDB\Server\3.4\bin\mongod.exe"