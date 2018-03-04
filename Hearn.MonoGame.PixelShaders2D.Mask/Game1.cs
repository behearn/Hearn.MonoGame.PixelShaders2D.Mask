using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace Hearn.MonoGame.PixelShaders2D.Mask
{
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        
        Texture2D _image;
        Texture2D _maskTexture;
        Effect _maskEffect;

        Vector2 _position;
        float _scale = 1f;
        float _rotation = 0f;
        
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            base.Initialize();

            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 854;
            graphics.ApplyChanges();

            _position = new Vector2(Window.ClientBounds.Width / 2, Window.ClientBounds.Height / 2);
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            _image = Content.Load<Texture2D>("pexels-photo-278961");
            _maskTexture = Content.Load<Texture2D>("StarMask");
            _maskEffect = Content.Load<Effect>("Mask");
        }
        
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            var elapsed = gameTime.ElapsedGameTime.Milliseconds;
            var total = gameTime.TotalGameTime.TotalMilliseconds;

            _scale = ((float)Math.Sin(elapsed * total / 10000f) * 8f);
            _rotation = (float)(elapsed * total / 10000f);
            
            base.Update(gameTime);
        }
        
        protected override void Draw(GameTime gameTime)
        {            
            GraphicsDevice.SetRenderTarget(null);
            GraphicsDevice.Clear(Color.CornflowerBlue);
            
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);

            spriteBatch.Draw(_image, new Vector2(0, 0), Color.Gray); //background image

            ApplyMask(_image, _maskTexture, _position, _scale, _rotation);
            spriteBatch.Draw(_image, new Vector2(0, 0), Color.White);

            spriteBatch.End();

            base.Draw(gameTime);
        }

        void ApplyMask(Texture2D baseTexture, Texture2D maskTexture, Vector2 maskCenter, float scale, float rotation)
        {
            _maskEffect.Parameters["BaseWidth"].SetValue((float)baseTexture.Width);
            _maskEffect.Parameters["BaseHeight"].SetValue((float)baseTexture.Height);

            _maskEffect.Parameters["MaskTexture"].SetValue(maskTexture);
            _maskEffect.Parameters["MaskWidth"].SetValue((float)maskTexture.Width);
            _maskEffect.Parameters["MaskHeight"].SetValue((float)maskTexture.Height);

            _maskEffect.Parameters["MaskScale"].SetValue(scale);

            _maskEffect.Parameters["MaskRotation"].SetValue(rotation);

            _maskEffect.Parameters["MaskCenterX"].SetValue((float)maskCenter.X);
            _maskEffect.Parameters["MaskCenterY"].SetValue((float)maskCenter.Y);

            _maskEffect.Techniques["Mask"].Passes[0].Apply();
        }
    }
}
