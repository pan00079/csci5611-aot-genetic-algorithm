// Genetic Algo by Kaushall

int num_agents = 20;
int num_parameters = 2; // Speed and Size
float agents[][];
Vec3 agent_pos[];
Vec3 agent_vel[];
float sucess_rates[];

void setup(){
  size(1280, 720);
  agents = new float[num_agents][num_parameters];
  sucess_rates = new float[num_agents];
  agent_pos = new Vec3[num_agents];
  agent_vel = new Vec3[num_agents];
  for(int i = 0; i < num_agents; i++){
    agents[i][0] = random(10, 100);
    agents[i][1] = random(50, 150);
    agent_pos[i] = new Vec3(1000, random(0, 700), 0);
    agent_vel[i] = new Vec3(-agents[i][0], 0, 0);
    sucess_rates[i] = 0.0;
  }
}

void update(float dt){
  keyPressed();
  for(int i = 0; i < num_agents; i++){
    agent_pos[i] = agent_pos[i].plus(agent_vel[i].times(dt));
    if(agent_pos[i].x <= 0){
      agent_vel[i] = new Vec3(0,0,0);
    }
  }
  float vel_sum = 0;
  float sucess_sum = 0;
  for(int i = 0; i < num_agents; i++){
    vel_sum += agent_vel[i].length();
    sucess_rates[i] = 1280 - agent_pos[i].x;
    sucess_sum += sucess_rates[i];
  }
  
  // When all the titans have been stopped
  if(vel_sum <= 0){
    for(int i = 0; i < num_agents; i++){
      sucess_rates[i] = sucess_rates[i]/sucess_sum;
      
    }
    setup();
    
  }
  
}

void keyPressed(){
   if(mousePressed){
     for(int i = 0; i < num_agents; i++){
       float dist = agent_pos[i].minus(new Vec3(mouseX, mouseY, 0)).length();
       if(dist < agents[i][1] && agent_vel[i].length() > 0){
          agent_vel[i] = new Vec3(0,0,0); 
          break;
       }
     }
   }
}

void draw(){
  update(1/frameRate);
  
  background(255);
  fill(0, 100, 200);
  for(int i = 0; i < num_agents; i++){
    circle(agent_pos[i].x, agent_pos[i].y, agents[i][1]);
  }
}
