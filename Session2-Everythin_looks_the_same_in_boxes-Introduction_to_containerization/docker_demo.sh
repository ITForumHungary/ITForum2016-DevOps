#!/bin/bash

COLS=80                                                                                                       
                                                                                                              
DEBUG=1                                                                                                       
                                                                                                              
normal_text="\e[0m"                                                                                           
bold_text="\033[1m"                                                                                           
info_color="\e[33m"                                                                                           
bold_underlined_text="\e[1;4m"
error_color="\e[31m"                                                                                          
debug_color="\e[1;33m"

demo_colorize(){
	demo_msg_type=${1} 

	color=''                                                                                        

	if [ "$demo_msg_type" == "COMM" ];then                                                                     
        color=${bold_underlined_text}                                                                                   
    fi                                                                                                        
                                                                                                              
    if [ "$demo_msg_type" == "OUT" ];then                                                                      
        color=${info_color}                                                                                  
    fi	

	if [ "$demo_msg_type" == "NORMAL" ];then                                                                     
        color=${normal_text}                                                                                  
    fi	

	echo -e ${color}
}

demo_banner() {                                                                                                
    demo_msg=${1}                                                                                              
    text_length=${#demo_msg}                                                                                   
                                                                                                              
    printf "${bold_text}+%.0s" $(seq 1 ${COLS}); printf "\n"                                                  
    printf "+%*s%*s\n" $[$COLS/2+${text_length}/2] "${demo_msg}" $[40-${text_length}/2-1] "+"                  
    printf "+%.0s" $(seq 1 ${COLS}); printf "\n${normal_text}"                                                
}

declare -a MSGS=(
	"List current images on host"
	"Search for a docker image in registry"
	"Get busybox docker image"
	"Run a simple command"
	"Check if container is still running"
	"Check in stopped containers"
	"Run container as a service"
	"Attach to that container"
	"Check running containers"
	"Kill and remove all containers"
	"Build a new image: run a container"
	"Build a new image: add file to the running container"
	"Build a new image: check the file"
	"Build a new image: kill the container"
	"Build a new image: commit changes to a new image"
	"Build a new image: check image layers"
	"Build a new image: remove old/original container"
	"Check current images"
	"Show layer problems: run new image"
	"Show layer problems: add new file to the container"
	"Show layer problems: remove old FILE1"
	"Show layer problems: kill container"
	"Show layer problems: commit changes to a new image"
	"Show layer problems: show changes in the container"
	"Show layer problems: show history of the new image"
	"Show layer problems: remove container"
	"Show layer problems: show images"
	"Show layer problems: run new image"
	"Show layer problems: check the files"
	"Tag change: show images"
	"Tag change: add tag"
    "Push image to local registry"	
)

declare -a COMMS=(
	"docker images"
	"docker search busybox"
	"docker pull busybox"
	"docker run --name busybox_0 busybox /bin/echo hello"
	"docker ps"
	"docker ps -a"
	"docker run -d --name busybox_1 busybox /bin/sh -c \"while true; do sleep 5000; done\""
	"docker exec -t -i busybox_1 /bin/sh"
	"docker ps -a"
	"docker kill busybox_0 busybox_1; docker rm busybox_0 busybox_1"
	"docker run -d --name busybox_1 busybox /bin/sh -c \"while true; do sleep 5000; done\""
	"docker exec -t -i busybox_1 /bin/sh -c \"dd if=/dev/zero of=/FILE1 bs=1024 count=10\""
	"docker exec -t -i busybox_1 /bin/sh -c \"ls -alh /FILE1\""
	"docker kill busybox_1"
	"docker commit busybox_1 my_busybox:v1"
	"docker history my_busybox:v1"
	"docker rm busybox_1"
	"docker images | grep busybox"
	"docker run -d --name busybox_2 my_busybox:v1 /bin/sh -c \"while true; do sleep 5000; done\""
	"docker exec -t -i busybox_2 /bin/sh -c \"dd if=/dev/zero of=/FILE2 bs=1024 count=10\""
	"docker exec -t -i busybox_2 /bin/sh -c \"rm -rf /FILE1\""
	"docker kill busybox_2"
	"docker commit busybox_2 my_busybox:v2"
	"docker diff busybox_2"
	"docker history my_busybox:v2"
	"docker rm busybox_2"
	"docker images |grep bosybox"
	"docker run -d --name busybox_3 my_busybox:v2 /bin/sh -c \"while true; do sleep 5000; done\""
	"docker exec -t -i busybox_3 /bin/sh -c \"ls -alh /|grep FILE\""
	"docker images"
	"docker tag my_busybox:v2 localhost:5000/my_busybox:latest"
	"docker push localhost:5000/mybusybox:latest"
)	

COUNT=${#MSGS[@]}

for ((i=0; i<$COUNT; i++));do
	clear
	MSG=${MSGS[i]}
	COMM=${COMMS[i]}
	demo_banner "${MSG}"
	demo_colorize COMM 
	echo "command: ${COMM}"
	demo_colorize NORMAL
	demo_colorize OUT
	script -q -c "${COMM}"
	demo_colorize NORMAL
	echo -e "\n\n"
	echo "---- hit ENTER to continue ----"
	read
done

