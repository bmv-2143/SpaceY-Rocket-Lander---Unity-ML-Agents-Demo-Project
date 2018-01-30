# Training

## Setup training environment

Use Tensorflow\Tensorboard\Python environment from
[Unity ML-Agents](https://github.com/Unity-Technologies/ml-agents) repository
to train SpaceY simulation.

## Hyperparameters

After training with the following Hyperparameters and Curriculum file (see below)
the algorithm was able to achieve rather good results (~91-94% success rate on
  32.8x20.5 game field).

"FrameToSkip" parameter in the Academy in Unity Project was set to 8.

```python
### General parameters
max_steps = 40e6 # Set maximum number of steps to run environment.
run_path = "rocket_lander/n1_run_17_new_model_ext1" # The sub-directory name for model and summary statistics
load_model = True # Whether to load a saved model.
train_model = True # Whether to train the model.
summary_freq = 10000 # Frequency at which to save training statistics.
save_freq = 20000 # Frequency at which to save model.
env_name = "rocket_lander" # Name of the training environment file.
curriculum_file = "rocket_lander_curriculum_data_run_17.json"
#curriculum_file = None

### Algorithm-specific parameters for tuning
gamma = 0.99 # Reward discount rate.
lambd = 0.95 # Lambda parameter for GAE.
time_horizon = 2048 # How many steps to collect per agent before adding to buffer.
beta = 0.3e-2 # Strength of entropy regularization
# beta = 1e-2 # also worked, but training went much slower
num_epoch = 5 # Number of gradient descent steps per batch of experiences.
num_layers = 2 # Number of hidden layers between state/observation encoding and value/policy layers.

# previous value epsilon = 0.2
epsilon = 0.3 # Acceptable threshold around ratio of old and new policy probabilities.
buffer_size = 10400 # How large the experience buffer should be before gradient descent.
learning_rate = 8e-5 # Model learning rate.

hidden_units = 64 # Number of units in hidden layer.
batch_size = 20 # How many experiences per gradient descent update step.
normalize = True

### Logging dictionary for hyperparameters
hyperparameter_dict = {'max_steps':max_steps, 'run_path':run_path, 'env_name':env_name,
    'curriculum_file':curriculum_file, 'gamma':gamma, 'lambd':lambd, 'time_horizon':time_horizon,
    'beta':beta, 'num_epoch':num_epoch, 'epsilon':epsilon, 'buffe_size':buffer_size,
    'leaning_rate':learning_rate, 'hidden_units':hidden_units, 'batch_size':batch_size}
```

## Curriculum file

rocket_lander_curriculum_data_run_17.json

```json
{
    "measure" : "reward",
    "thresholds" :
	[-0.1, -0.1, -0.1, -0.1, -0.1, -0.1, -0.1, -0.1, -0.1],
    "min_lesson_length" : 4,
    "signal_smoothing" : true,
    "parameters" :
    {
        "rocket_start_pos_border_x" :
		[0.8, 0.8, 0.8, 0.8, 0.8, 0.8, 0.8, 0.8, 0.8, 0.8],

        "rocket_start_pos_border_y" :
		[0.5, 0.5, 0.5, 0.5, 0.5, 0.5, 0.5, 0.5, 0.5, 0.5],

	"board_scale_x" :  
		[18.4, 20.0, 21.6, 23.2, 24.8, 26.4, 28.0, 29.6, 31.2, 32.8],

	"board_scale_y" :
		[11.5, 12.5, 13.5, 14.5, 15.5, 16.5, 17.5, 18.5, 19.5, 20.5]
    }
}
```
