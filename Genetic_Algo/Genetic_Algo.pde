// Genetic Algo by Kaushall

int num_agents = 10;
int num_best_agents = 1;  // should be 0.1 * num_agents
int num_parameters = 3; // Speed and Size and Color
float maxSpeed = 300;
float minSpeed = 10;
float maxSize = 100;
float minSize = 10;
float agents[][];
float best_agents[][]; 
Vec3 agent_pos[];
Vec3 agent_vel[];
float sucess_rates[];
float best_sucess_rates[];

void setup(){
  size(1280, 720);
  agents = new float[num_agents][num_parameters];
  best_agents = new float[num_best_agents][num_parameters];
  sucess_rates = new float[num_agents];
  best_sucess_rates = new float[num_best_agents];
  agent_pos = new Vec3[num_agents];
  agent_vel = new Vec3[num_agents];
  for(int i = 0; i < num_agents; i++){
    for(int j = 0; j < num_parameters; j++){
      agents[i][j] = random(10, 100);
    }
    agent_pos[i] = new Vec3(1000, random(0, 700), 0);
    agent_vel[i] = new Vec3(-agents[i][0], 0, 0);
    sucess_rates[i] = 0.0;
  }
}

void find_best_agents(){
  int best_value;
  float[][] temp_agents = new float[num_agents][num_parameters];
  float[] temp_sucess_rates = new float[num_agents];
  float swap_holder;
  
  // Insertion Sort on temp agents
  for(int i = 0; i < num_agents; i++){
    for(int j = 0; j < num_parameters; j++){
        temp_agents[i][j] = agents[i][j];
    }
    temp_sucess_rates[i] = sucess_rates[i];
  }
  for(int i = 0; i < num_agents-1; i++){
    best_value = i;
    for(int j = i+1; j < num_agents; j++){
        if(sucess_rates[best_value] < sucess_rates[j]){
          best_value = j;
        }
    }
    for(int k = 0; k < num_parameters; k++){
          swap_holder = temp_agents[i][k];
          temp_agents[i][k] = temp_agents[best_value][k];
          temp_agents[best_value][k] = swap_holder;
    }
    swap_holder = temp_sucess_rates[i];
    temp_sucess_rates[i] = temp_sucess_rates[best_value];
    temp_sucess_rates[best_value] = swap_holder;
  }
  
  for(int i = 0; i < num_best_agents; i++){
    for(int j = 0; j < num_parameters; j++){
      best_agents[i][j] = temp_agents[i][j];
    }
    best_sucess_rates[i] = temp_sucess_rates[i];
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
    find_best_agents();
    
    float[] average_of_parameter = new float[num_parameters];
    for(int i = 0; i < num_parameters; i++){
      average_of_parameter[i] = 0;
      for(int k = 0; k < num_best_agents; k++){
        average_of_parameter[i] += best_agents[k][i];
      }
      average_of_parameter[i] = average_of_parameter[i]/num_parameters;
    }
    
    
    
    
    for(int i = 0; i < num_agents; i++){
      for(int j = 0; j < num_parameters; j++){
        agents[i][j] = average_of_parameter[j] + random(-20, 20);
      }
      
      // Setting MaxLimits and MinLimits
      if(agents[i][0] > maxSpeed){
        agents[i][0] = maxSpeed;
      }
      if(agents[i][1] > maxSize){
        agents[i][1] = maxSize;
      }
      if(agents[i][0] < minSpeed){
        agents[i][0] = maxSpeed;
      }
      if(agents[i][1] < minSize){
        agents[i][1] = minSize;
      }
      if(agents[i][2] < minSpeed){
        agents[i][2] = maxSpeed;
      }
      if(agents[i][2] < minSize){
        agents[i][2] = minSize;
      }
      
      // Resseting the simulation
      agent_pos[i] = new Vec3(1000, random(0, 700), 0);
      agent_vel[i] = new Vec3(-agents[i][0], 0, 0);
      sucess_rates[i] = 0.0;
    }
    
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
  
  fill(255, 0, 0);
  strokeWeight(10);
  line(50, 0, 50, 720);
  
  strokeWeight(1);
  fill(0, 100, 200);
  for(int i = 0; i < num_agents; i++){
    circle(agent_pos[i].x, agent_pos[i].y, agents[i][1]);
  }
}
