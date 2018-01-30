public class RocketLanderAcademy : Academy {

    public float boardScaleX = 16;
    public float boardScaleY = 10;

    public float rocketStartPosBorderX;
    public float rocketStartPosBorderY;

    public override void InitializeAcademy()
    {
        UpdateCurriculumLearningParams();
    }

    public override void AcademyReset()
	{
        UpdateCurriculumLearningParams();
    }

    private void UpdateCurriculumLearningParams()
    {
        boardScaleX = (float)resetParameters["board_scale_x"];
        boardScaleY = (float)resetParameters["board_scale_y"];

        rocketStartPosBorderX = (float)resetParameters["rocket_start_pos_border_x"];
        rocketStartPosBorderY = (float)resetParameters["rocket_start_pos_border_y"];
    }

    //   public override void AcademyStep()
    //{
    //}

    public void SetScreenToHd()
    {
        inferenceConfiguration = new ScreenConfiguration(1280, 720, 5, 1.0f, 60);
    }

    public void SetScreenToFullHd()
    {
        inferenceConfiguration = new ScreenConfiguration(1920, 1080, 5, 1.0f, 60);
    }
}
