#!/bin/bash

# Configuration
CONFIGURATION="Release"
DAEMON_EXE="AcquireServer"
DAEMON_FOLDER="/home/cdl"
DAEMON_PATH="$DAEMON_FOLDER/$DAEMON_EXE"
SERVICE_NAME="cdl2024-server.service"
SERVICE_PATH="/etc/systemd/system/$SERVICE_NAME"
REMOTE_USER="cdl"
REMOTE_HOST="opicdl"
ARCH="linux-arm"

# Environment variables
ASPNETCORE_URLS="http://0.0.0.0:8080"
ASPNETCORE_ENVIRONMENT="Development"
#ASPNETCORE_ENVIRONMENT="Production"

build_daemon() {
    echo "Building daemon executable..."
    dotnet publish -c "$CONFIGURATION" -r $ARCH --self-contained true -p:PublishSingleFile=true
}

# Function to upload the daemon executable
upload_daemon() {
    echo "Uploading daemon executable..."
    rsync -avz "./bin/$CONFIGURATION/net8.0/$ARCH/publish/$DAEMON_EXE" "$REMOTE_USER@$REMOTE_HOST:$DAEMON_PATH"
}

# Function to create the systemd service file
create_service() {
    echo "Creating systemd service file..."
    ssh "$REMOTE_USER@$REMOTE_HOST" "sudo tee $SERVICE_PATH > /dev/null" <<EOF
[Unit]
Description=CDL2024 AcquireServer Service
After=network.target

[Service]
ExecStart=$DAEMON_PATH
Restart=always
User=$REMOTE_USER
Environment="ASPNETCORE_URLS=$ASPNETCORE_URLS"
Environment="ASPNETCORE_ENVIRONMENT=$ASPNETCORE_ENVIRONMENT"

[Install]
WantedBy=multi-user.target
EOF
}

# Function to reload systemd and restart the service
restart_service() {
    echo "Reloading systemd and restarting the service..."
    ssh "$REMOTE_USER@$REMOTE_HOST" "sudo systemctl daemon-reload"
    ssh "$REMOTE_USER@$REMOTE_HOST" "sudo systemctl restart $SERVICE_NAME"
    ssh "$REMOTE_USER@$REMOTE_HOST" "sudo systemctl enable $SERVICE_NAME"
}

# Function to verify the service status
verify_service() {
    echo "Verifying service status..."
    ssh "$REMOTE_USER@$REMOTE_HOST" "sudo systemctl status $SERVICE_NAME"
    ssh "$REMOTE_USER@$REMOTE_HOST" "sudo journalctl -fu $SERVICE_NAME"  
}

# Main script
build_daemon
upload_daemon
create_service
restart_service
verify_service
